

```SKELL
varables are things that you can have string replaced on
[vars]
varA=Var A Value
[endvars]
endvars is used to tell the skell program that we are done with varables in this file.


files are things that will be written to the filesystem after doing string replacement on the skell files.
[file FileName.txt]
I Will be written into the file!
the value of varA is "{varA}"
[eof]
eof is used to show 'end of file'

```

