#include <assert.h>
#include <stdint.h>
#include <stdio.h>

int popcount_1_data(uint64_t x) {
  int c = 0;
  for (int i = 0; i < 64; i++) {
    //1 × 0 = 0 and 0 × 0 = 0
    c += x & 1;
    x >>= 1;
  }
  return c;
}

int popcount_1_control(uint64_t x) {
  int c = 0;
  for (int i = 0; i < 64; i++) {
    if (x & 1)
      c++;
    x >>= 1;
  }
  return c;
}

static const int pop4[16] = {0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4};

int popcount_4_data(uint64_t  x) {
  int c = 0;
  for (int i = 0; i < 16; i++) {
    c += pop4[x & 0xf];
    x >>= 4;
  }
  return c;
}

int popcount_4_control(uint64_t x) {
  int c = 0;
  for (int i = 0; i < 16; i++) {
    switch (x & 0xf) {
    case 0:
      c += 0;
      break;
    case 1:
      c += 1;
      break;
    case 2:
      c += 1;
      break;
    case 3:
      c += 2;
      break;
    case 4:
      c += 1;
      break;
    case 5:
      c += 2;
      break;
    case 6:
      c += 2;
      break;
    case 7:
      c += 3;
      break;
    case 8:
      c += 1;
      break;
    case 9:
      c += 2;
      break;
    case 10:
      c += 2;
      break;
    case 11:
      c += 3;
      break;
    case 12:
      c += 2;
      break;
    case 13:
      c += 3;
      break;
    case 14:
      c += 3;
      break;
    case 15:
      c += 4;
      break;
    }
    x >>= 4;
  }
  return c;
}

static int pop8[256];

int popcount_8_data(uint64_t x) {
  int c = 0;
  for (int i = 0; i < 8; i++) {
    c += pop8[x & 0xff];
    x >>= 8;
  }
  return c;
}

static int pop16[256 * 256];

static void init(void) {
  for (int i = 0; i < 256 * 256; i++) {
    if (i < 256)
      pop8[i] = __builtin_popcount(i);
    pop16[i] = __builtin_popcount(i);
  }
}

int popcount_16_data(uint64_t x) {
  int c = 0;
  for (int i = 0; i < 4; i++) {
    c += pop16[x & 0xffff];
    x >>= 16;
  }
  return c;
}

int popcount_kernighan(uint64_t n) {
  int count = 0;
  while (n) {
    count += n & 0x1u;
    n >>= 1;
  }
  return count;
}

// next three are from wikipedia:
// https://en.wikipedia.org/wiki/Hamming_weight

//types and constants used in the functions below
//uint64_t is an unsigned 64-bit integer variable type (defined in C99 version of C language)
static const uint64_t m1  = 0x5555555555555555; //binary: 0101...
static const uint64_t m2  = 0x3333333333333333; //binary: 00110011..
static const uint64_t m4  = 0x0f0f0f0f0f0f0f0f; //binary:  4 zeros,  4 ones ...
static const uint64_t m8  = 0x00ff00ff00ff00ff; //binary:  8 zeros,  8 ones ...
static const uint64_t m16 = 0x0000ffff0000ffff; //binary: 16 zeros, 16 ones ...
static const uint64_t m32 = 0x00000000ffffffff; //binary: 32 zeros, 32 ones
static const uint64_t h01 = 0x0101010101010101; //the sum of 256 to the power of 0,1,2,3...

int popcount64a(uint64_t x) {
    x = (x & m1 ) + ((x >>  1) & m1 ); //put count of each  2 bits into those  2 bits
    x = (x & m2 ) + ((x >>  2) & m2 ); //put count of each  4 bits into those  4 bits
    x = (x & m4 ) + ((x >>  4) & m4 ); //put count of each  8 bits into those  8 bits
    x = (x & m8 ) + ((x >>  8) & m8 ); //put count of each 16 bits into those 16 bits
    x = (x & m16) + ((x >> 16) & m16); //put count of each 32 bits into those 32 bits
    x = (x & m32) + ((x >> 32) & m32); //put count of each 64 bits into those 64 bits
    return x;
}

int popcount64b(uint64_t x) {
    x -= (x >> 1) & m1;             //put count of each 2 bits into those 2 bits
    x = (x & m2) + ((x >> 2) & m2); //put count of each 4 bits into those 4 bits
    x = (x + (x >> 4)) & m4;        //put count of each 8 bits into those 8 bits
    x += x >>  8;  //put count of each 16 bits into their lowest 8 bits
    x += x >> 16;  //put count of each 32 bits into their lowest 8 bits
    x += x >> 32;  //put count of each 64 bits into their lowest 8 bits
    return x & 0x7f;
}

int popcount64c(uint64_t x) {
    x -= (x >> 1) & m1;             //put count of each 2 bits into those 2 bits
    x = (x & m2) + ((x >> 2) & m2); //put count of each 4 bits into those 4 bits
    x = (x + (x >> 4)) & m4;        //put count of each 8 bits into those 8 bits
    return (x * h01) >> 56;         //returns left 8 bits of x + (x<<8) + (x<<16) + (x<<24) + ...
}

int popcount64_fast(uint64_t x) {
  return __builtin_popcountl(x);
}

static int (*popcounts[])(uint64_t) = {
  popcount_1_data,
  popcount_1_control,
  popcount_4_data,
  popcount_4_control,
  popcount_8_data,
  popcount_16_data,
  popcount_kernighan,
  popcount64a,
  popcount64b,
  popcount64c,
  popcount64_fast,
  0
};

#include <time.h>
#include <fcntl.h>
#include <stdint.h>
#include <stdlib.h>
#include <unistd.h>

static uint64_t rand64(void) {
  uint64_t rv;
  int fd = open("/dev/urandom", O_RDONLY);
  if (fd < 0)
    abort();
  size_t count = read(fd, &rv, sizeof(rv));
  if (count != sizeof(rv))
    abort();
  if (close(fd) != 0)
    abort();
  return rv;
}

static double timespecDiff(struct timespec *timeA_p,
                           struct timespec *timeB_p) {
  return
    ((timeA_p->tv_sec * 1000000000.0) + timeA_p->tv_nsec) -
    ((timeB_p->tv_sec * 1000000000.0) + timeB_p->tv_nsec);
}

static void test_popcounts(void) {
  const int N = 10000;
  uint64_t nums[N];
  nums[0] = 0;
  nums[1] = 0xffffffffffffffffUL;
  for (int i = 2; i < N; ++i)
    nums[i] = rand64();
  int last_sum;
  for (int j = 0; popcounts[j]; ++j) {
    int sum = 0;
    struct timespec start, end;
    clock_gettime(CLOCK_MONOTONIC, &start);
    for (int i = 0; i < N; ++i)
      sum += popcounts[j](nums[i]);
    clock_gettime(CLOCK_MONOTONIC, &end);
    double timeElapsed = timespecDiff(&end, &start);
    double speed = timeElapsed / N;
    printf("%lf ns / op\n", speed);
    if (j != 0)
      assert(sum == last_sum);
    last_sum = sum;
  }
}

int main(void) {
  init();
  test_popcounts();
  return 0;
}
