CS 6015
Popcount Performance Assignment
Kayla Cresswell

————————————————————————————————————————————————
C# file
In terminal: brew install mono (from home directory) 
————————————————————————
In the project file: 

mcs popCount.cs
->This will generate an .exe file

Mono popCount.exe
->This will show you your file output

———————————— My output ——————————————
makaylas-mbp:pcC# mcresswell$ mcs pc_cSharp.cs 

makaylas-mbp:pcC# mcresswell$ ls

pc_cSharp.cs	pc_cSharp.exe

makaylas-mbp:pcC# mcresswell$ mono pc_cSharp.exe 

popcount1 
9E-05 ms / op
popcount2 
1E-05 ms / op
popcount3 
0 ms / op


————————————————————————————————————————————————
Java file  
————————————————————————
Terminal: 
Navigate to project directory

javac PopCount.java 
->This compiles it

java PopCount
->This runs it and shows you the output


———————————— My output ——————————————
makaylas-mbp:pcJava mcresswell$ javac PopCount.java 

makaylas-mbp:pcJava mcresswell$ java PopCount

popcount1 125.77488 ns / op
popcount2 48.31064 ns / op
popcount3 10.98549 ns / op

————————————————————————————————————————————————

Javascript

In terminal from home directory: brew install node 

————————————————————————

Terminal: 

Navigate to project directory

node popcount.js

———————————— My output ——————————————

makaylas-mbp:pcJavaScript mcresswell$ node popcount.js 

popcount1 
0.0005 ns / op
popcount2 
0.0002 ns / op
popcount3 
0.0002 ns / op

————————————————————————————————————————————————

Python

Terminal: 

Navigate to project directory
python popcount.py

———————————— My output ——————————————

makaylas-mbp:pcPython mcresswell$ python popcount.py 

popcount1 
0.000003 ms / op
popcount2 
0.000002 ms / op
popcount3 
0.000001 ms / op

