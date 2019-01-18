#!/usr/bin/python
import time
import sys
import random

modes = [1, 2, 3, 0]

  #136.400000 ns / op
  #popcount_1_data
def popcount1(x):
  c = 0
  for i in range(16):
    num = (x & 0xf)
    if num == 0:
      c += 0
    elif num == 1:
      c += 1
    elif num == 2:
      c += 1
    elif num == 3:
      c += 2
    elif num == 4:
      c += 1
    elif num == 5:
      c += 2
    elif num == 6:
      c += 2
    elif num == 7:
      c += 3
    elif num == 8:
      c += 1
    elif num == 9:
      c += 2
    elif num == 10:
      c += 2
    elif num == 11:
      c += 3
    elif num == 12:
      c += 2
    elif num == 13:
      c += 3
    elif num == 14:
      c += 3
    elif num == 15:
      c += 4
    x >>= 4

  return c

#30.900000 ns / op
def popcount2(n):
  count = 0
  while n != 0:
    count += (n & 0x1)
    n >>= 1
  return count

#binary: 0101010101010101
m1 = 0x5555555555555555

#binary: 001100110011001100110011
m2 = 0x3333333333333333

#binary: 000011110000111100001111
m4 = 0x0f0f0f0f0f0f0f0f

#sum of 256 raised to 1, 2, 3, 4, ...
h01 = 0x0101010101010101

#fastest function 3.300000 ns / op
def popcount3(x):
  x -= (x >> 1) & m1 #
  x = (x & m2) + ((x >> 2) & m2)
  x = (x + (x >> 4)) & m4
  return ((x * h01) >> 56)

def main():
  N = 100
  nums = []
  nums.append(0)
  nums.append(sys.maxsize)
  for i in range(2, N):
    nums.append(random.randint(1, 1000))
  lastSum = 0
  j = 0
  while modes[j] != 0:
    sum = 0
    if j == 0:
      print("popcount1 ")
    elif j == 1:
      print("popcount2 ")
    elif j == 2:
      print("popcount3 ")
    else:
      print("Invalid Function")

    startTime = time.clock()
    for i in range(0, N):
      if j == 0:
        sum += popcount1(nums[i])
      elif j == 1:
        sum += popcount2(nums[i])
      elif j == 2:
        sum += popcount3(nums[i])

    endTime = time.clock()
    timeElapsed = endTime - startTime
    speed = timeElapsed / N
    print("%f ms / op" % (speed))
    j += 1

if __name__ == "__main__":
  main()
