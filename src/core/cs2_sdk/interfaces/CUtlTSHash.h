#pragma once

#include <array>
#include <cstdint>
#include <vector>
using UtlTsHashHandleT = std::uint64_t;

class CUtlMemoryPool {
  public:
    // returns number of allocated blocks
    int BlockSize() const {
        return m_blocks_per_blob_;
    }
    int Count() const {
        return m_block_allocated_size_;
    }
    int PeakCount() const {
        return m_peak_alloc_;
    }
  private:
    std::int32_t m_block_size_ = 0; // 0x0558
    std::int32_t m_blocks_per_blob_ = 0; // 0x055C
    std::int32_t m_grow_mode_ = 0; // 0x0560
    std::int32_t m_blocks_allocated_ = 0; // 0x0564
    std::int32_t m_block_allocated_size_ = 0; // 0x0568
    std::int32_t m_peak_alloc_ = 0; // 0x056C
};

template <class T, class Keytype = std::uint64_t>
class CUtlTSHash {
  public:
    // Invalid handle.
    static UtlTsHashHandleT InvalidHandle(void) {
        return static_cast<UtlTsHashHandleT>(0);
    }

    // Returns the number of elements in the hash table
    [[nodiscard]] int BlockSize() const {
        return m_entry_memory_.BlockSize();
    }
    [[nodiscard]] int Count() const {
        return m_entry_memory_.Count();
    }

    // Returns elements in the table
    std::vector<T> GetElements(void);
  public:
    // Templatized for memory tracking purposes
    template <typename DataT>
    struct HashFixedDataInternalT {
        Keytype m_ui_key;
        HashFixedDataInternalT<DataT>* m_next;
        DataT m_data;
    };

    using HashFixedDataT = HashFixedDataInternalT<T>;

    // Templatized for memory tracking purposes
    template <typename DataT>
    struct HashFixedStructDataInternalT {
        DataT m_data;
        Keytype m_ui_key;
        char pad_0x0020[0x8];
    };

    using HashFixedStructDataT = HashFixedStructDataInternalT<T>;

    struct HashStructDataT {
        char pad_0x0000[0x10]; // 0x0000
        std::array<HashFixedStructDataT, 256> m_list;
    };

    struct HashAllocatedDataT {
      public:
        auto GetList() {
            return m_list_;
        }
      private:
        char pad_0x0000[0x18]; // 0x0000
        std::array<HashFixedDataT, 128> m_list_;
    };

    // Templatized for memory tracking purposes
    template <typename DataT>
    struct HashBucketDataInternalT {
        DataT m_data;
        HashFixedDataInternalT<DataT>* m_next;
        Keytype m_ui_key;
    };

    using HashBucketDataT = HashBucketDataInternalT<T>;

    struct HashUnallocatedDataT {
        HashUnallocatedDataT* m_next_ = nullptr; // 0x0000
        Keytype m_6114; // 0x0008
        Keytype m_ui_key; // 0x0010
        Keytype m_i_unk_1; // 0x0018
        std::array<HashBucketDataT, 256> m_current_block_list; // 0x0020
    };

    struct HashBucketT {
        HashStructDataT* m_struct_data = nullptr;
        void* m_mutex_list = nullptr;
        HashAllocatedDataT* m_allocated_data = nullptr;
        HashUnallocatedDataT* m_unallocated_data = nullptr;
    };

    CUtlMemoryPool m_entry_memory_;
    HashBucketT m_buckets_;
    bool m_needs_commit_ = false;
};

template <class T, class Keytype>
std::vector<T> CUtlTSHash<T, Keytype>::GetElements(void) {
    std::vector<T> list;

    const int n_count = Count();
    auto n_index = 0;
    auto& unallocated_data = m_buckets_.m_unallocated_data;
    for (auto element = unallocated_data; element; element = element->m_next_) {
        for (auto i = 0; i < BlockSize() && i != n_count; i++) {
            list.emplace_back(element->m_current_block_list.at(i).m_data);
            n_index++;

            if (n_index >= n_count)
                break;
        }
    }
    return list;
}