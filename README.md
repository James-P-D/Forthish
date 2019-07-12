# Forthish
A C# Console application for a Forth-like stack-based language

## Introduction

[Forth](https://en.wikipedia.org/wiki/Forth_(programming_language)) is an low-level, imperative stack-based language, invented in the late 1960s. Due to its small size and closeness to assembler, the language was often used for robotics where microprocessor memory was minimal.

The language is rarely used nowadays, although it has heavily influenced the desktop publishing language PostScript, and a number of Golfing languages such as [CJam](https://esolangs.org/wiki/CJam), [05AB1E](https://github.com/Adriandmen/05AB1E) and [GolfScript](http://www.golfscript.com/golfscript/).

This implemention is by no means complete, nor matching the specific syntax of most Forth specifications. This decision is based on the somewhat unorthodox choice of keywords in most traditional implementions, most obviously the choice of using `if..else..endif` rather than the traditional syntax of `if..else..then` (where `then` is actually an `endif`) which might confuse novice users. Other design decisions include introducing a `::` operator for redefining definitions, whereas in classic Forth, this isn't possible, with definitions only ever being set once.

For a more complete Forth implementation I'd recommend the [FastForth](https://osdn.net/projects/fast-forth/) compiler/interpreter and [J.V. Noble's A Beginner's Guide to Forth](http://galileo.phys.virginia.edu/classes/551.jvn.fall01/primer.htm)

## Contents

1. Basics
2. Output String
3. Stack Operators
4. Maths Operators
5. Logical Operators
6. Definitions
7. If..else..endif
8. Loop..endloop and Repeat..until
9. Variables, Values and Constants
10. Arrays
11. Example Programs

## Basics

This Forth interpreter has been implemented as an interactive console application. After running you should see something like the following:

```
Forth Interpreter
Press <ESC> to exit

Loading resource: Constants
Loading resource: Definitions

Ready!
```

It is also possible to load Forth source-code files by passing them as paramters to the executable:

```
C:\>Forth.exe c:\source\code.for
Forth Interpreter
Press <ESC> to exit

Loading resource: Constants
Loading resource: Definitions
Loading source file: c:\source\code.for

Ready!
```

As already mentioned in the introduction, Forth is a stack-based language and most programming will consist of pushing values to the stack and then calling various operators which will operate on the stack, usually pushing the result of the operation back onto the stack.

We can add items to the stack simply by typing numbers and pressing enter:

```
2019
```

The interpreter will respond with the following which indicates that the process was successful:

```
> OK
```

We can pop items from the stack and display them using `.`:

```
.
> 2019 OK
```

We can push multiple items on a single line:

```
1 2 3 4 5
> OK
```

..and we can pop them back off in a single line:

```
. . . . .
> 5 4 3 2 1 OK
```

Note that because of the last-in-first-out nature of stacks, the numbers are read back to us in the opposite order to which they were added.

We can only pop items from the stack if the stack is not empty. If we attempt to pop more items than there are available, we'll see a stack underflow exception:

```
11 22 33 . . . .
> 33 22 11 ERROR: Stack Underflow
```

In the above we successfully push values to the stack, then used the `.` operator to pop and display them. The interpreter was able to pop and display the first three values, but when it tried for the fourth item, we generated an exception.

We can also push and pop in a single line:

```
1 2 3 . . 4 5 . . .
> 3 2 5 4 1 OK
```

In Forth comments are places between parentheses:

```
1 2 (3 4) 5 . . .
> 5 2 1 OK
```

In the above example, 1 and 2 are pushed to the stack, but then the interpretor encounters a comment, so it stops processing until the brackets are closed. Having reached the end of the comment, the interpreter continues and pushes the number 5 before popping each item and displaying it.

Usually Forth will process input as soon as enter is pressed. If you wish to write a multi-line program you must use the `\` character to terminate each line. The interpreter will not begin execution until a line is entered which doesn't terminate in `\`:

```
1 2 3 4 \
5 6 7 8 \
. . . . \
. . . .
> 8 7 6 5 4 3 2 1 OK
```

So far we have only been pushing integers to our stack, but it is also possible to push floats, characters and hexadecimal numbers using the commands `fractional`, `char`, and `hex`. For example:

```
hex bd34 .
> BD34 OK
0a0c .
> A0C OK
fractional
> OK
12.34 .
> 12.34 OK
char
> OK
'd' .
> d OK
decimal 1234 .
> 1234 OK
```

Because Forth stores everything in 32-bit cells, how it displays the information popped from the stack will depend upon what mode you are in. If you aren't careful you may see some strange output if in the wrong number-format:

```
fractional 12.34
> OK
decimal .
> 1095069860 OK
char 'A'
> OK
decimal .
> 65 OK
hex abcd
> OK
decimal .
> 43981 OK
```

## Output String

We have already seen how to pop and display the top of the stack using the `.` operator, but you may also find you need to display strings. This is accomplished with the `.""` operator. For example:

```
."Hello, world! "
> Hello, world! OK
```

We can also add new-lines with the carriage-return command `cr`:

```
cr ."Hello, " cr ."world!" cr
>
Hello,
world!
OK
```

## Stack Operators

Since Forth is a stack-based language, there are multiple built-in operators for manipulating the stack.

`dup` duplicates the top value on the stack and pushes it again:

```
1 2 3 4 dup . . . . .
> 4 4 3 2 1 OK
```

`swap` swaps the top two items on the stack:

```
1 2 3 4 swap . . . .
> 3 4 2 1 OK
```

`drop` deletes the top of the stack:

```
1 2 3 4 drop . . .
> 3 2 1 OK
```

`rot` rotates the top three items:

```
1 2 3 4 rot . . .
> 2 4 3 1 OK
```

`over` duplicates the second item on the stack and pushes it to the top:

```
1 2 3 4 over . . . . .
> 3 4 3 2 1 OK
```

`tuck` duplicates the top item on the stack and pushes it below the second:

```
1 2 3 4 tuck . . . . .
> 4 3 4 2 1 OK
```

`pick` accepts a parameter `N` which is then used to duplicate the Nth element onto the top of the stack. Note that `0 pick` is effectively the same as `rot`:

```
1 2 3 4 5 6 7 8 9 10
> OK
5 pick
> OK
. . . . . . . . . . .
> 5 10 9 8 7 6 5 4 3 2 1 OK
```

`roll` accepts a paramter `N` which is then removed from the stack and placed at the top:

```
1 2 3 4 5 6 7 8 9 10
> OK
3 roll . . . . . . . . . .
> 7 10 9 8 6 5 4 3 2 1 OK
```

## Maths Operators

There are multiple mathematical operators which pop two items from the stack, perform an operation, and push the result back to the stack:

```
20 15 + .
> 35 OK
```

You will need to think carefully about the order in which you push numbers and operators in Forth since we have no concept of operator-precedence (multiplication binds closer than addition in most C-like languages), and no option of using parentheses. If we wished to compute (3 + 4) * (-5 + 13) we would need to enter the following:

```
3 4 + -5 13 + * .
> 56 OK
```

What's happening here? First we push 3 and 4, then the addition operator pops and adds them together before putting the result (7) on the stack. Then -5 and 13 are pushed and another operator pops them, adds them, and pushes the result (8) to the stack. Finally, the multiplication operator pops our two calculations, multiplies them and puts the final result 56) back on the stack before popping it and displaying it.

We can also do integer division/modulus:

```
13 5 /
> 2 OK
13 5 mod
> 3 OK
```

## Logical Operators

In Forth, `true` is considered to be zero and `false` as `-1`. Infact, both `true` and `false` are defined an constants in the interpreter (more on Constants, Variables and Values later on.) We can confirm the values of `true` and `false` easily enough:

```
true .
> 0 OK
false .
> -1 OK
```

We also have the basic equality operators `=`, `!=`, `<`, `>`, `<=`, `>=`, all of which pop two items from the stack and push the result:

```
3 4 = .
> -1 OK
3 4 != .
> 0 OK
3 4 < .
> -1 OK
```

Finally, we have the usual logical operators `and`, `or` and `not` respectively:

```
true true and .
> 0 OK
true false and .
> -1 OK
true false or .
> 0 OK
false false or .
> -1 OK
true not .
> 0 OK
false not .
> -1 OK
```

## Definitions

For operations that occur frequently, we can create definitions. These take the form `: N C1..CN ;` where N is the name of the definition, and C1 to CN are a series of commands.

For example, to create a definition that squares a number we would write:

```
: square dup * ;
> OK
```

Our definition duplicates whatever is on the stack and adds it to the stack, then the multiplication operator pops the top two values, multiples them, and pushes the result. To square the value 3 and then display it we would use the following:

```
3 square .
> 9 OK
```

We can even call the same definition multiple times:

```
3 square square .
> 81 OK
```

The above pushes 3 to the stack, then our definition squares it and pushes the result (9) to the stack, then the definition is called again and 9 is popped, squared, and the result (81) pushed. Finally we pop the final result and display it.

Definitions can also call other definitions, so we can easily make a definition for cubing values:

```
: cubed square square ;
```

We can then use the following:

```
3 cubed .
> 81 OK
```

If you want to change a definition, you must use the `::` operator:

```
: cubed dup * dup * ;
> ERROR: 'cubed' is already defined
:: cubed dup * dup * ;
> OK
3 cube .
> 81 OK
```

It is also possible to call definitions recursively, but watch out for overflowing the processing stack. For example, the following definition adds 1 to the item on the stack, displays it, then calls itself again:

```
: add1 1 + dup . add1 ;
```

If we now push zero to the stack and call `add1`, we see the following:

```
0 add1
> 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 64 65 66 67 68 69 70 71 72 73 74 75 76 77 78 79 80 81 82 83 84 85 86 87 88 89 90 91 92 93 94 95 96 97 98 99 100 ERROR: Processing stack overflow
```

We will learn how to avoid this by using base cases for recursive calls to definitions when we discuss the `if..else..endif` commands.

Finally, it is possible to get details of all existing definitions with the `viewdefinitions` command.

## If..else..endif

The `if..endif` construct pops a value from the stack, and if true will execute all commands until it reaches the `endif`:

```
: ispositive 0 > if ."positive" cr endif ;
> OK
5 ispositive
> positive OK
-5 ispositive
> OK
```

When we run the program it correctly outputted `positive` for 5 but it did nothing when `if` evaluated to false. If we want to execute commands when the `if` fails, we can use `else`:

```
:: ispositive 0 > if ."positive" else ."negative" endif cr ;
> OK
5 ispositive
> positive OK
-5 ispositive
> negative OK
```

It is possible to have `if..else..if..` ladders:

```
:: ispositive              \
dup 0 < if                 \
        ."positive " drop  \
else 0 > if                \
            ."negative "   \
        else               \
            ."zero "       \
	endif              \
endif ;
> OK
-5 ispositive
> negative OK
5 ispositive
> positive OK
0 ispositive
> zero OK
```

Finally, it is possible to have `if`s within other `if`s:

```
: size                           \
dup 10 > if                      \
        dup 5 > if               \
                ."very small "   \
        else                     \
                ."quite small "  \
        endif                    \
else                             \
        dup 15 > if              \
                ."quite big "    \
        else                     \
                ."very big "     \
        endif                    \
endif                            \
drop ;
> OK
2 size
> very small OK
7 size
> quite small OK
12 size
> quite big OK
99 size
> very big OK
```

## Loop..endloop and Repeat..until

`loop..endloop` constucts accept three parameters specifying the end condition, the start condition, and the increment:

```
10 0 2 loop swap dup . swap endloop
> 0 2 4 6 8 OK
```

`repeat..until` constructs repeat a sequence of commands until `true` is on the stack:

```
5 repeat ." hello! " 1 - dup 0 = until drop
>  hello!  hello!  hello!  hello!  hello! OK
```

In the above code, we push 5 to the stack and then repeat a sequence of commands that decrements the counter until it is zero. The final `drop` removes the counter.

## Variables, Values and Constants

So far, all literal objects we have created have been put on the stack. For global objects such as variables, values, constants and arrays, we will need to allocate space in memory whenever they are declared. This Forth implementation has been designed for 32-bit memory allocations, which means all values will be stored in 4 byte chunks called a `cell`. We will need to know the size of cells quite frequently, so there is an in-built constant at our disposal:

```
cell .
> 4 OK
```

We also have a command called `here` which will push to the stack the current memory pointer. When you first interrogate the value of `here`, you will be told where the current memory pointer is:

```
here .
> 16 OK
```

`here` will continue to be 16 until we start declaring objects, at which point we should start to see the pointer increment. More on that shortly..

For many programming applications we may need values which do not change throughout the duration of the program. To achieve this we use the `constant` keyword which pops a value from the stack and must then be proceeded by a valid name:

```
20 constant twenty
> OK
```

Having created our constant, we can retrieve our value by pushing it to the stack and displaying it:

```
twenty .
> 20 OK
```

Since constants should not change their value during the running of a program, attempting to assign another value to a constant, will cause an error:

```
30 constant twenty
> ERROR: 'twenty' is already defined
```

Having created our constant, if we interrogate `here` again, we will see that the memory pointer has been increased by the size of 1 cell (4 bytes):

```
here .
> 20 OK
```

For creating named-objects whose values can change, we can use variables. To declare a variable called `my_var` we would use:

```
variable my_var
> OK
```

Again, if we interrogate `here` we will see that the memory pointer has been incremented again:

```
here.
> 24 OK
```

Note that when we declared our variable we didn't give it any default value. If we push the variable to the stack and then pop it's value, we might expect it to be zero, but instead we get the following:

```
my_var .
> 20 OK
```

This is because the value of the variable is the pointer to a memory location, in this case 20, which was by no small coincidence, the value of `here` before we declared `my_var`.

To set the value of the cell pointed to by a variable, we use the `!` (store) command which expects two values on the stack specifying the value to store, and the memory location of the variable:

```
1234 my_var !
> OK
```

In the above code, we push the value 1234 to the stack, then push the value of `my_var` (which is the memory location 20) and then call `!` to store the value in memory.

To retrieve the value of a cell in memory, we can use the `@` (fetch) command, which pops a memory location from the stack, retrieves the value for that cell, and then pushes it to the stack:

```
my_var @ .
> 1234 OK
```

In the above code, we push the value of `my_var` (the memory location 20) to the stack, then `@` pops it, grabs the value of the cell at this location, and pushes it to the stack. Finally, we pop the value off and display it.

If we don't care about memory locations, we can use `value`s instead of variables. The syntax for creating values is the same as for creating constants:

```
2019 value this_year
> OK
```

We can then push the value of `this_year` to the stack and display it:

```
this_year .
> 2019 OK
```

We can change the value of `this_year` with `to`:

```
2020 to this_year
> OK
this_year .
> 2020 OK
```

Finally you can view all constants, variables and values with `viewobjects`.

## Arrays

As already discussed, entering `variable some_name` pushes the next available memory location to the stack. For the creation of arrays, Forth offers the `cells` operator which takes one parameter and multiplies it by the size of `cell` and pushes the result to the stack. Forth also offers the `alloc` operator which takes two paramters, first the initial memory location, and then the number of bytes to allocate.

Using these two operators we can allocate a 6-cell array using the following:

```
variable my_array 5 cells allot 
> OK
```

In the above, `variable my_array` pushes the next available memory location to the stack, then 5 is pushed, then `cells` pops the number 5 and memory location from the stack, multiplies them and pushes the result to the stack. Finally, `allot` allocates the memory.

We can now assign values to each cell in our array:

```
11 my_array 0 cells + ! \
22 my_array 1 cells + ! \
33 my_array 2 cells + ! \
44 my_array 3 cells + ! \
55 my_array 4 cells + ! \
66 my_array 5 cells + ! 
> OK
```

...and retrieve the values:

```
my_array 0 cells + @ . \
my_array 1 cells + @ . \
my_array 2 cells + @ . \
my_array 3 cells + @ . \
my_array 4 cells + @ . \
my_array 5 cells + @ . 
> 11 22 33 44 55 66 OK
```

## Example Programs

As previously mentioned, it is possible to call definitions recursively, but we need to have a base-case to ensure there isn't infinite recursion. Below is an example definition for calculating a factorial:

```
:: factorial dup 1 > if dup 1 - factorial * else drop 1 endif ;
> OK
5 factorial .
> 120 OK
```