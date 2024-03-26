

```SKELL
[vars]
ask projectName
somethingElse="10"
[endvars]

[file Makefile]
MODULE={projectName}
... other stuff for the make file ...
[eof]

[file tb_{projectName}.cpp]
#some c++ stuff
#include <stdlib.h> ... and whatever
#include "V{projectName}.h"
#include "V{projectName}___024root.h"

int main(int argc, char **argv, char **env){    
    V{projectName} *dut = new V{projectName};
    ... and other stuff
}
[eof]


[file="{projectName}.sv"]
module {projectName} (
    input wire clk
);

endmodule;
[eof]
```

