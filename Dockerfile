FROM registry.gitlab.steamos.cloud/steamrt/sniper/sdk:latest

WORKDIR /workspace

RUN apt update && apt install -y \
    clang-16 \
    cmake \
    ninja-build \
    git \
    zlib1g-dev \
    libssl-dev \
    libprotobuf-dev \
    protobuf-compiler \
    pkg-config \
    curl && \
    ln -sf /usr/bin/clang-16 /usr/bin/clang && \
    ln -sf /usr/bin/clang++-16 /usr/bin/clang++
