var mode = [1, 2, 3, 0];

//136.400000 ns / op
//popcount_1_data
var popcount1 = function (x) {
    var c = 0;
    for (var i = 0; i < 16; i++) {
      var num = (x & 0xf);
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

//middle
//30.900000 ns / op
var popcount2 = function (n) {
    var count = 0;
    while (n != 0) {
      count += (n & 0x1);
      n >>= 1;
    }
    return count;
}

//fastest
// 3.300000 ns / op
//binary: 0101010101010101
var m1  = 0x5555555555555555;

//binary: 001100110011001100110011
var m2  = 0x3333333333333333;

//binary: 000011110000111100001111
var m4  = 0x0f0f0f0f0f0f0f0f;

//sum of 256 raised to 1, 2, 3, 4, ...
var h01 = 0x0101010101010101;


//fastest
// 3.300000 ns / op
var popcount3 = function(x) {
    x -= (x >> 1) & m1; // put count of each 2 bits into those 2 bits
    x = (x & m2) + ((x >> 2) & m2); // put count of each 4 bits into those 4 bits
    x = (x + (x >> 4)) & m4; // put count of each 8 bits into those 8 bits
    // static const uint64_t h01 = 0x0101010101010101; //the sum of 256 to the power
    // of 0,1,2,3...
    return ((x * h01) >> 56); // returns left 8 bits of x + (x<<8) + (x<<16) + (x<<24) + ...
}

var main = function() {
    var N = 10000;
    var nums = [];
    nums[0] = 0;
    nums[1] = Number.MAX_VALUE;
    // Random random = new Random();
    for (var i = 2; i < N; ++i) {
    	var rand = Math.floor(Math.random() * 1000000000);
    	nums[i] = rand;
    }
    var lastSum = 0;
    for (var j = 0; mode[j] != 0; ++j) {
      var sum = 0;
      if (j === 0) {
        console.log("popcount1 ");
      } else if (j === 1) {
        console.log("popcount2 ");
      } else if (j === 2) {
      	console.log("popcount3 ");
      } else {
        console.log("Invalid Function");
      }

      var startTime = new Date();
      for (var i = 0; i < N; ++i) {
        if (j === 0) {
          sum += popcount1(nums[i]);
        } else if (j == 1) {
          sum += popcount2(nums[i]);
        } else if (j == 2) {
          sum += popcount3(nums[i]);
        }
      }

      var endTime = new Date();
      var timeElapsed = endTime - startTime;
      var speed = timeElapsed / N;
      console.log(speed + " ns / op");
    }
}

main()
