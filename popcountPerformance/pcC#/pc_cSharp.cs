using System;
using System.Diagnostics;

namespace MyNamespace{
public class PopCount {

  static int[] mode = new int[] {1, 2, 3, 0};

  //136.400000 ns / op
  //popcount_1_data
  static int popcount1(long x) {
    int c = 0;
    for (int i = 0; i < 16; i++) {
      int num = (int)(x & 0xf);
      switch (num) {
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

  //30.900000 ns / op
  static int popcount2(long n) {
    long count = 0;
    while (n != 0) {
      count += (n & 0x1);
      n >>= 1;
    }
    return (int)count;
  }

  //fastest
  // 3.300000 ns / op
  static int popcount3(long x) {
    //binary: 0101010101010101
  	long m1  = 0x5555555555555555L;

    //binary: 001100110011001100110011
    long m2  = 0x3333333333333333L;

    //binary: 000011110000111100001111
    long m4  = 0x0f0f0f0f0f0f0f0fL;

    //sum of 256 raised to 1, 2, 3, 4, ...
    long h01 = 0x0101010101010101L;

    // put count of each 2 bits into 2 bit x
    x -= (x >> 1) & m1;

    // put count of each 4 bits into 4 bit x
    x = (x & m2) + ((x >> 2) & m2);

    // put count of each 8 bits into 8 bit x
    x = (x + (x >> 4)) & m4;

    // returns left 8 bits of x + (x<<8) + (x<<16) + (x<<24) + (x<<32)  ...
    return (int) ((x * h01) >> 56);
  }

  public static void Main() {
    int num = 100000;
    long[] nums = new long[num];
    nums[0] = 0;
    nums[1] = long.MaxValue;
    Random rnd = new Random();
    for (int i = 2; i < num; ++i)
      nums[i] = rnd.Next(1, 1000000000);
    for (int j = 0; mode[j] != 0; ++j) {
      int sum = 0;
      if (j == 0) {
        System.Console.WriteLine("popcount1 ");
      } else if (j == 1) {
        System.Console.WriteLine("popcount2 ");
      } else if (j == 2) {
        System.Console.WriteLine("popcount3 ");
      } else {
        System.Console.WriteLine("Invalid Function");
      }

      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int i = 0; i < num; ++i) {
        if (j == 0) {
          sum += popcount1(nums[i]);
        } else if (j == 1) {
          sum += popcount2(nums[i]);
        } else if (j == 2) {
          sum += popcount3(nums[i]);
        }
      }
      stopwatch.Stop();
      double elapsedMS = stopwatch.ElapsedMilliseconds;
      double speed = elapsedMS / num;
      System.Console.WriteLine(speed + " ms / op");
    }

  }
}
}
