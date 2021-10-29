# SpaceSimulator

A classical mechanics code library written in C# for calculating gravity, laws of motion, thrust and angular momentum.

Written by Gabriel Rayo and Lance Gulinao of Group GUMARATI from STEM-12C of DLSU-IS Laguna

---

# Documentation

Class properties and method names in bold typeface are static

## Double2

The library stores positional data, velocity, forces using Double2, a datatype which stores of x and y components.

**Constructor parameters**
Parameter name | Datatype | optional | Description  
 ------------- | -------- | -------- | ----------
\_x | double | false | the x component of the instance
\_y | double | false | the y component of the instance

**Properties**
name | Datatype | Description  
 ----------- | ----------- | ----------
x | double | the x component of the instance
y | double | the y component of the instance
SquareMagnitude | double | the sum of the squares of the x and y components
Magnitude | double | the square root of the SquareMagnitude, uses the Pythagorean Theorem
**Zero** | Double2 | Double2 whose x and y properties are both 0
**One** | Double2 | Double2 whose x and y properties are both 1
**Perpendicular** | Double2 | Double2 whose x and y properties are -1 and 1 respectively
