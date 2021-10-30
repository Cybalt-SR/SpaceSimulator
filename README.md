# SpaceSimulator

A classical mechanics code library written in C# for calculating gravity, laws of motion, thrust and angular momentum.

Written by Gabriel Rayo and Lance Gulinao of Group GUMARATI from STEM-12C of DLSU-IS Laguna

-   [SpaceSimulator](#spacesimulator)
-   [Documentation](#documentation)
    -   [Double2 (struct)](#double2-struct)
        -   [Constructor Parameters](#constructor-parameters)
        -   [Properties](#properties)
        -   [Methods](#methods)
    -   [Double (struct)](#double-struct)
        -   [Methods](#methods-1)
    -   [SpaceSimulation (static class)](#spacesimulation-static-class)
        -   [Properties](#properties-1)
    -   [TrajectoryData (struct)](#trajectorydata-struct)
        -   [Constructor Parameters](#constructor-parameters-1)
        -   [Properties](#properties-2)

---

# Documentation

Properties and method names in bold typeface are static

## Double2 (struct)

The library stores positional data, velocity, forces using Double2, a datatype which stores of x and y components.

### Constructor Parameters

| Parameter name | Datatype | optional | Description                     |
| -------------- | -------- | -------- | ------------------------------- |
| \_x            | double   | false    | the x component of the instance |
| \_y            | double   | false    | the y component of the instance |

### Properties

| name              | Datatype | Description                                                |
| ----------------- | -------- | ---------------------------------------------------------- |
| **Zero**          | Double2  | Double2 whose x and y properties are both 0                |
| **One**           | Double2  | Double2 whose x and y properties are both 1                |
| **Perpendicular** | Double2  | Double2 whose x and y properties are -1 and 1 respectively |
| x                 | double   | the x component of the instance                            |
| y                 | double   | the y component of the instance                            |
| SquareMagnitude   | double   | the sum of the squares of the x and y components           |
| Magnitude         | double   | Total displacement of the x and y components               |
| Normalized        | Double2  | The components of the Double2 divided by their magnitude   |

### Methods

**Lerp** - applies linear interpolation to the components of two Double2 instances

| Parameter name | Datatype | optional | Description                                                                           |
| -------------- | -------- | -------- | ------------------------------------------------------------------------------------- |
| a              | double2  | false    | First dobule2 instance                                                                |
| b              | double2  | false    | Second dobule2 instance                                                               |
| t              | double   | false    | Percentage between 0-1 that represents the distance between the components of a and b |

**DirFromAngle** - Converts an angle theta to a Double2 representing the x and y directions

| Parameter name | Datatype | optional | Description                                                       |
| -------------- | -------- | -------- | ----------------------------------------------------------------- |
| angle          | double   | false    | The engle in degrees that the point should be at from the +x-axis |

ToString - Shows the x and y components in a better format for console debugging

## Double (struct)

Contains methods useful for working with doubles

### Methods

**Lerp** - applies linear interpolation between two doubles

| Parameter name | Datatype | optional | Description                                                         |
| -------------- | -------- | -------- | ------------------------------------------------------------------- |
| a              | double   | false    | First double                                                        |
| b              | double   | false    | Second double                                                       |
| t              | double   | false    | Percentage between 0-1 that represents the distance between a and b |

## SpaceSimulation (static class)

A static class that stores universal constants for use in calculations

### Properties

| name       | Datatype | Description                                             |
| ---------- | -------- | ------------------------------------------------------- |
| **gconst** | double   | The gravitational constant used for calculating gravity |

## TrajectoryData (struct)

Holds partial information about a TrajectoryBody's trajectory

### Constructor Parameters

| Parameter name  | Datatype | optional | Description                                      |
| --------------- | -------- | -------- | ------------------------------------------------ |
| position        | Double2  | false    | the position of the body in absolute coordinates |
| velocity        | Double2  | false    | the velocity of the body                         |
| angle           | double   | false    | the angle at which the body is facing at         |
| angularVelocity | double   | false    | the body's rate of turn                          |
| Mass            | double   | false    | the mass of the body                             |

### Properties

| name            | Datatype | Description                                                             |
| --------------- | -------- | ----------------------------------------------------------------------- |
| Force           | Double2  | The horizontal and vertical forces being applied to th e rajectory body |
| mass            | double   | Readonly: the mass of the body                                          |
| Pos             | Double2  | the position of the body in absolute coordinates                        |
| Velocity        | Double2  | the velocity of the body                                                |
| Angle           | double   | the angle at which the body is facing at                                |
| AngularVelocity | double   | the body's rate of turn                                                 |
