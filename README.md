# SpaceSimulator

A classical mechanics code library written in C# for calculating gravity, laws of motion, thrust and angular momentum.

Written by Gabriel Rayo and Lance Gulinao of Group GUMARATI from STEM-12C of DLSU-IS Laguna

- [SpaceSimulator](#spacesimulator)
- [Documentation](#documentation)
	- [Double2 (struct)](#double2-struct)
		- [Constructor Parameters](#constructor-parameters)
		- [Properties](#properties)
		- [Methods](#methods)
	- [Double (struct)](#double-struct)
		- [Methods](#methods-1)
	- [SpaceSimulation (static class)](#spacesimulation-static-class)
		- [Properties](#properties-1)
	- [TrajectoryData (struct)](#trajectorydata-struct)
		- [Constructor Parameters](#constructor-parameters-1)
		- [Properties](#properties-2)
		- [Methods](#methods-2)
	- [TrajectoryBody (class)](#trajectorybody-class)
		- [Constructor Parameters](#constructor-parameters-2)
		- [Properties](#properties-3)
		- [Methods](#methods-3)
	- [ThrustBody (class, extends TrajectoryBody)](#thrustbody-class-extends-trajectorybody)
		- [Constructor Parameters](#constructor-parameters-3)
		- [Properties](#properties-4)
		- [Methods](#methods-4)
	- [CelestialBody (class, extends TrajectoryBody)](#celestialbody-class-extends-trajectorybody)
		- [Constructor Parameters](#constructor-parameters-4)
		- [Properties](#properties-5)
		- [Methods](#methods-5)

---

# Documentation

## Double2 (struct)

The library stores positional data, velocity, forces using Double2, a datatype which stores of x and y components.

### Constructor Parameters

| Parameter | Datatype | optional | Description                     |
| --------- | -------- | -------- | ------------------------------- |
| \_x       | double   | false    | the x component of the instance |
| \_y       | double   | false    | the y component of the instance |

### Properties

| Name                       | Datatype | Description                                                |
| -------------------------- | -------- | ---------------------------------------------------------- |
| **Zero (static)**          | Double2  | Double2 whose x and y properties are both 0                |
| **One (static)**           | Double2  | Double2 whose x and y properties are both 1                |
| **Perpendicular (static)** | Double2  | Double2 whose x and y properties are -1 and 1 respectively |
| x                          | double   | the x component of the instance                            |
| y                          | double   | the y component of the instance                            |
| SquareMagnitude            | double   | the sum of the squares of the x and y components           |
| Magnitude                  | double   | total displacement of the x and y components               |
| Normalized                 | Double2  | the components converted to values between 0 and 1         |
| Angle                      | double   | the angle of the vector from the positive x-axis           |

### Methods

**Lerp (static)** - applies linear interpolation to the components of two Double2 instances

| Parameter | Datatype | optional | Description                                                                           |
| --------- | -------- | -------- | ------------------------------------------------------------------------------------- |
| a         | double2  | false    | First dobule2 instance                                                                |
| b         | double2  | false    | Second dobule2 instance                                                               |
| t         | double   | false    | Percentage between 0-1 that represents the distance between the components of a and b |

returns: A new Double2 instance whose components are lerped from the inputs

**DirFromAngle (static)** - Converts an angle theta to a Double2 representing the x and y directions

| Parameter | Datatype | optional | Description                                                       |
| --------- | -------- | -------- | ----------------------------------------------------------------- |
| angle     | double   | false    | The angle in degrees that the point should be at from the +x-axis |

returns: A Double2 that represents a point that is theta degrees away from the +x-axis

**LerpKeyList (static)** - Applies linear interpolation to an array of Double2 and returns the y component

| Parameter | Datatype        | optional | Description                                                               |
| --------- | --------------- | -------- | ------------------------------------------------------------------------- |
| list      | List\<Double2\> | false    | List of Double2, where x is the second and y is a percentage              |
| time      | double          | false    | The index that will be used to determine the thrust from the list of keys |

returns: The y-component of the Double2 at a given second. If that specific second is not found, linear interpolation is applied.

Example:

```cs
// Let list be [[0, 0], [1800, 1], [3600, 0.5]]
// No thrust at 0 seconds
// Full thrust at 1800 seconds or 30 minutes
// Half thrust at 3600 hour or 60 minutes

Double2.LerpKeyList(list, 1800); // returns 1
Double2.LerpKeyList(list, 3600); // returns 0.5
Double2.LerpKeyList(list, 2700); // returns 0.75 since 2700 is halfway between 1800 and 3600
```

**convertSimpleKeysToLerpable (static)** - Converts an array of thrust keys into a format that the code library can work with

| Parameter  | Datatype | optional | Description                                                                                 |
| ---------- | -------- | -------- | ------------------------------------------------------------------------------------------- |
| simpleKeys | double[] | false    | An array of doubles ranging between 0-1 where their index represents time in the simulation |

returns: A List\<Double2\> whose y component is the thrust at a given time, which is the x component.

**MagnitudeSigfig** - Formats the magnitude and direction of the Double2 into a string that has a specified amount of decimal places

| Parameter | Datatype | optional | Description                                   |
| --------- | -------- | -------- | --------------------------------------------- |
| sigfig    | int      | false    | The amount of significant figures to be shown |
| unit      | string   | false    | The units to be printed                       |

**ToStringSigFig** - Formats the x and y values to a string that has a specified amount of decimal places

| Parameter | Datatype | optional | Description                                   |
| --------- | -------- | -------- | --------------------------------------------- |
| sigfig    | int      | false    | The amount of significant figures to be shown |

## Double (struct)

Contains methods that are useful for working with doubles

### Methods

**Lerp (static)** - applies linear interpolation between two doubles

| Parameter | Datatype | optional | Description                                                         |
| --------- | -------- | -------- | ------------------------------------------------------------------- |
| a         | double   | false    | First double                                                        |
| b         | double   | false    | Second double                                                       |
| t         | double   | false    | Percentage between 0-1 that represents the distance between a and b |

returns: double between a and b with linear interpolation applied based on t

**ToSigFigs (static)** - Formats a double to have only one whole number and a specified amount of decimal places

| Parameter   | Datatype | optional | Description                                                          |
| ----------- | -------- | -------- | -------------------------------------------------------------------- |
| value       | double   | false    | The double to format                                                 |
| sigfig      | int      | false    | Number of decimal places to show                                     |
| shiftAmount | int      | true     | The number of decimal places that the function shifted the double to |

**ToStringSigFig (static)** - Converts a double to a certain number of decimal places, and formats it similar to scientific notation

| Parameter | Datatype | optional | Description                      |
| --------- | -------- | -------- | -------------------------------- |
| value     | double   | false    | The double to format             |
| sigfig    | int      | false    | Number of decimal places to show |

## SpaceSimulation (static class)

A static class that stores universal constants for use in calculations

### Properties

| Name          | Datatype | Description                                             |
| ------------- | -------- | ------------------------------------------------------- |
| **gconst**    | double   | The gravitational constant used for calculating gravity |
| **maxTorque** | double   | theoretical maximum turning force of a rocket           |

## TrajectoryData (struct)

Holds partial information about a TrajectoryBody's trajectory

### Constructor Parameters

| Parameter       | Datatype | optional | Description                                      |
| --------------- | -------- | -------- | ------------------------------------------------ |
| Mass            | double   | false    | the mass of the body                             |
| position        | Double2  | false    | the position of the body in absolute coordinates |
| velocity        | Double2  | false    | the velocity of the body                         |
| angle           | double   | false    | the angle at which the body is facing at         |
| angularVelocity | double   | false    | the body's rate of turn                          |

### Properties

| Name            | Datatype | Description                                                             |
| --------------- | -------- | ----------------------------------------------------------------------- |
| mass            | double   | Readonly: the mass of the body                                          |
| Pos             | Double2  | the position of the body in absolute coordinates                        |
| Velocity        | Double2  | the velocity of the body                                                |
| Force           | Double2  | The horizontal and vertical forces being applied to the trajectory body |
| Angle           | double   | the angle at which the body is facing at                                |
| AngularVelocity | double   | the body's rate of turn                                                 |
| Torque          | double   | The torque that is being applied on the body                            |

### Methods

**GetMass** - returns the mass of the rocket

**PrintToConsole** - prints the properties of the TrajectoryData instance to the console

## TrajectoryBody (class)

Represents any object in space

### Constructor Parameters

| Parameter              | Datatype               | optional | Description                                                      |
| ---------------------- | ---------------------- | -------- | ---------------------------------------------------------------- |
| startingTrajectoryData | TrajectoryData         | false    | The initial trajectory data of the trajectoryBody                |
| Trajectory             | List\<TrajectoryData\> | true     | A precalculated trajectory                                       |
| interval               | int                    | true     | The time interval between TrajectoryData snapshots. 1 by default |

### Properties

| Name                  | Datatype       | Description                                                              |
| --------------------- | -------------- | ------------------------------------------------------------------------ |
| name                  | string         | The name of this trajectory body                                         |
| percentOffset         | double         | The offset that the Trajectory is shifted by along time                  |
| SimulationSecond      | int            | The time in seconds since the simulation has started                     |
| CurrentTrajectoryData | TrajectoryData | The current trajectory data of the body, used as cache for CalculateNext |

### Methods

**InitCalculation** - Resets the trajectoryBody back to a default state

| Parameter              | Datatype       | Optional | Description                                       |
| ---------------------- | -------------- | -------- | ------------------------------------------------- |
| startingTrajectoryData | TrajectoryData | false    | The initial trajectory data of the trajectoryBody |

**CalculateNext** - advances the simulation by 1 second and performs collision detection

| Parameter    | Datatype        | Optional | Description                                                         |
| ------------ | --------------- | -------- | ------------------------------------------------------------------- |
| otherObjects | CelestialBody[] | false    | Array of celestialObjects (planets) that will be taken into account |

**GetInterpolatedT** - Determines the lerp percentage and TrajectoryList indexes for a given second in the simulation

| Parameter | Datatype | Optional | Output | Description                                                                         |
| --------- | -------- | -------- | ------ | ----------------------------------------------------------------------------------- |
| time      | int      | false    | false  | Simulation time in seconds that will be used to calculate indexes                   |
| index     | int      | false    | true   | The lower TrajectoryList index, before the specified time                           |
| nextIndex | int      | false    | true   | The upper TrajectoryList index, after the specified time                            |
| clamped   | boolean  | true     | false  | If true, allows method to sometimes return first and last index. `false` by default |

returns: the lerp percentage between index and nextIndex

Example:

```cs
// the user wants to get the position of the rocket 1500 seconds in
double lerpPercentage = GetInterpolatedT(1500, out int index, out int nextIndex);
Double2 a = TrajectoryList[index].Pos; 						// the position of the rocket 1000 seconds in
Double2 b = Trajectorylist[nextIndex].Pos; 					// the position of the rocket 2000 seconds in
double lerpedPosition = Double2.Lerp(a, b, lerpPercentage); // the position of the rocket 1500 seconds in
```

**Get\<LerpableDouble2\>AtTime** - Gets the lerped value of the position, velocity, force of the trajectoryBody at a given second

| Parameter  | Datatype | Optional | Description                |
| ---------- | -------- | -------- | -------------------------- |
| timeSecond | int      | false    | Simulation time in seconds |

returns: Lerped Double2 property of the trajectoryBody at the requested time

**Get\<LerpableDouble\>AtTime** - Gets the lerped value of the angle, angularVelocity, Torque of the trajectoryBody at a given second

| Parameter  | Datatype | Optional | Description                |
| ---------- | -------- | -------- | -------------------------- |
| timeSecond | int      | false    | Simulation time in seconds |

returns: Lerped Double property of the trajectoryBody at the requested time

**GetCurrentForces** - Calculates the forces being applied on the TrajectoryBody with planets taken into account

This method is overwritten by classes that derive from TrajectoryBody to add forces specific to them.

| Parameter    | Datatype        | Optional | Description                                                         |
| ------------ | --------------- | -------- | ------------------------------------------------------------------- |
| otherObjects | CelestialBody[] | false    | Array of celestialObjects (planets) that will be taken into account |

returns: Double2 containing the forces being applied in the horizontal and vertical axis

**GetGravity** - Determines the forces of gravity acting on the TrajectoryBody in the horizontal and vertical axis

| Parameter      | Datatype        | Optional | Description                                                         |
| -------------- | --------------- | -------- | ------------------------------------------------------------------- |
| gravityHolders | CelestialBody[] | false    | Array of celestialObjects (planets) that will be taken into account |

returns: Double2 containing the forces of gravity being applied in the horizontal and vertical axis

**GetRawForceOfGravity** - Calculates the raw force of gravity between the two bodies

| Parameter | Datatype      | Optional | Description                              |
| --------- | ------------- | -------- | ---------------------------------------- |
| body      | CelestialBody | false    | The planet to compute gravity against    |
| bodyPos   | Double2       | false    | The position of the body at a given time |

returns: double representing the raw force of gravity between two bodies

**ConvertTorqueToDegPerSecond** - Converts torque in newton-meters to degrees/second

| Parameter | Datatype | Optional | Description                          |
| --------- | -------- | -------- | ------------------------------------ |
| torque    | double   | false    | the torque being applied to the body |
| length    | double   | false    | The length of the body               |

## ThrustBody (class, extends TrajectoryBody)

Represents a rocket

### Constructor Parameters

| Parameter              | Datatype               | optional | Description                                                               |
| ---------------------- | ---------------------- | -------- | ------------------------------------------------------------------------- |
| startingTrajectoryData | TrajectoryData         | false    | The initial trajectory data of the thrustBody                             |
| ThrustKeys             | double[]               | false    | double array that represents the rocket's linear thrust levels over time  |
| AngleKeys              | double[]               | false    | double array that represents the rocket's angular thrust levels over time |
| RocketLength           | double                 | false    | Length of the rocket                                                      |
| ExhaustVelo            | double                 | true     | The speed of the rocket's exhaust gases                                   |
| FuelBurnRate           | double                 | true     | The rocket's fuel burn rate                                               |
| Trajectory             | List\<TrajectoryData\> | true     | A precalculated trajectory                                                |
| interval               | int                    | true     | The time interval between TrajectoryData snapshots. 1 by default          |

### Properties

| Name         | Datatype | Description                             |
| ------------ | -------- | --------------------------------------- |
| rocketLength | double   | Length of the rocket                    |
| exhaustVelo  | double   | The speed of the rocket's exhaust gases |
| fuelBurnRate | double   | The rocket's fuel burn rate             |

### Methods

**GetCurrentForces** - Calculates the forces being applied on the TrajectoryBody with planets taken into account

This method overrides the GetCurrentForces method and adds thrust

| Parameter    | Datatype        | Optional | Description                                                         |
| ------------ | --------------- | -------- | ------------------------------------------------------------------- |
| otherObjects | CelestialBody[] | false    | Array of celestialObjects (planets) that will be taken into account |

returns: Double2 containing the forces being applied in the horizontal and vertical axis

**GetThrust** - Computes for the thrust of the rocket

| Parameter    | Datatype        | Optional | Description                                                            |
| ------------ | --------------- | -------- | ---------------------------------------------------------------------- |
| otherObjects | CelestialBody[] | false    | Array of planets, used to determine where the rocket is launching from |
| percentage   | double          | false    | The thrust level of the rocket                                         |

returns: The change in position of the rocket with its angle taken into account

**GetCurrentTorques** - calculates the torque being applied to the rocket at a given second

| Parameter    | Datatype        | Optional | Description                                                         |
| ------------ | --------------- | -------- | ------------------------------------------------------------------- |
| otherObjects | CelestialBody[] | false    | Array of celestialObjects (planets) that will be taken into account |

returns: the torque being applied to the rocket in newton-meters

**GetTorque** - calculates the torque being applied to the rocket

| Parameter  | Datatype | Optional | Description                                        |
| ---------- | -------- | -------- | -------------------------------------------------- |
| percentage | double   | false    | The angular thrust being applied in a given second |

returns: the torque being applied to the rocket in newton-meters

**GetLength** - returns the length of the rocket

**GetThrustKeys** - returns the linear thrust keys

**GetAngularthrustKeys** - returns the angular thrust keys

## CelestialBody (class, extends TrajectoryBody)

Represents a planet

### Constructor Parameters

| Parameter              | Datatype               | optional | Description                                                      |
| ---------------------- | ---------------------- | -------- | ---------------------------------------------------------------- |
| startingTrajectoryData | TrajectoryData         | false    | The initial trajectory data of the celestialBody                 |
| radius                 | double                 | false    | The radius of the planet                                         |
| Trajectory             | List\<TrajectoryData\> | true     | A precalculated trajectory                                       |
| interval               | int                    | true     | The time interval between TrajectoryData snapshots. 1 by default |

### Properties

| Name   | Datatype | Description              |
| ------ | -------- | ------------------------ |
| Radius | double   | The radius of the planet |

### Methods

**GetLength** - returns the diamted of the planet

**ChangeRadius** - changes the radius of the planet

| Name      | Datatype | Description                  |
| --------- | -------- | ---------------------------- |
| newRadius | double   | The new radius of the planet |
