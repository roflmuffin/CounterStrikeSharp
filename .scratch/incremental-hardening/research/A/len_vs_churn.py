#!/usr/bin/env python3
"""Spearman correlation between current signature length and historical churn (per platform). Caveat: current length is post-hoc (sigs were lengthened after breaking)."""
import re,sys
rows=[l for l in open(sys.argv[1]) if re.search(r' lin +\d+B',l)]
def rank(v):
    s=sorted(range(len(v)),key=lambda i:v[i]); r=[0]*len(v); i=0
    while i<len(s):
        j=i
        while j+1<len(s) and v[s[j+1]]==v[s[i]]: j+=1
        for k in range(i,j+1): r[s[k]]=(i+j)/2
        i=j+1
    return r
def sp(a,b):
    ra,rb=rank(a),rank(b); n=len(a); ma=sum(ra)/n; mb=sum(rb)/n
    num=sum((x-ma)*(y-mb) for x,y in zip(ra,rb)); den=(sum((x-ma)**2 for x in ra)*sum((y-mb)**2 for y in rb))**.5
    return num/den
L=[int(re.search(r'lin +(\d+)B',l).group(1)) for l in rows]; W=[int(re.search(r'win +(\d+)B',l).group(1)) for l in rows]
CL=[int(re.search(r'churn lin= *(\d+)',l).group(1)) for l in rows]; CW=[int(re.search(r'win= *(\d+) commits',l).group(1)) for l in rows]
print(f"n={len(rows)} spearman(len,churn) linux={sp(L,CL):.2f} windows={sp(W,CW):.2f}")
