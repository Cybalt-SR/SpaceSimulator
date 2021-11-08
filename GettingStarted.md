# Table of Contents

- [Table of Contents](#table-of-contents)
- [Getting Started](#getting-started)
- [Working with the Library](#working-with-the-library)
	- [Creating initial trajectory data](#creating-initial-trajectory-data)
	- [Creating planets](#creating-planets)
	- [Creating rockets](#creating-rockets)
	- [The actual calculation](#the-actual-calculation)

# Getting Started

To get started with the code library,
Install Visual Studio 2019 with support for .NET desktop development under the "Desktop & Mobile section"

# Working with the Library

This code library makes it easier for you to simulate the behaviour of rocket and planets in space

Rockets are represented using a class called ThrustBody, planets are represented using a class called CelestialBody, and their trajectory is represented using a struct called TrajectoryData.

These classes and structs allow you to simulate a wide range of scenarios that may occur in space. Don't worry, you don't need to know the difference between classes and structs in order to use this code library.

## Creating initial trajectory data

In order to create a rocket and planet, we first have to initiate their initial trajectory data, this will hold the initial information about their mass, position, velocity, angle, and angular velocity.

```cs
// the object will start 50 meters above and away from the origin
Double2 startingPos = new Double2(50, 50);
// and it won't have any horizontal or vertical velocity
Double2 startingVelocity = new Double2(0, 0);

TrajectoryData initialTrajectory = new TrajectoryData(
	mass, 					// mass of the object in kg
	startingPos, 			// a Double2 of the object's position
	startingVelcoity, 		// a Double2 of the object's velocity
	startingAngle,  		// angle of the object in degrees
	startingAngularVelocity // angle of the object in degrees/second
);
```

Generally, a Double2 is used to store information that can be broken down to horizontal and vertical components, like position, velocity, and acceleration

## Creating planets

We can then use the initial trajectory in creating planets:

```cs
CelestialBody earth = new CelestialBody(
	earthInitialTrajectory, // the initial trajectory of earth
	earthRadius // the radius of earth
);

// you can repeat the above code for as many planets as you want
// then we can create an array of CelestialBody to represent planets
CelestialBody[] planets = { earth };
```

## Creating rockets

We also use the initial trajectory to create rockets, but aside from that we also have to define the rocket's linear and angular thrust

```cs
//         Time |    1s |    2s |    3s |    4s |    5s |
// Thrust Level |    0% |   20% |   40% |   60% |   80% |
double[] thrustKeys = {0, 0.2, 0.4, 0.6, 0.8};

//         Time |    0s |    1s |    2s |    3s |    4s |
// Thrust Level |    0% |   10% |    0% |   20% |    0% |
//    Direction |  None |  Left |  None | Right |  None |
double[] angularThrustKeys = {0, 0.1, 0.0, -0.2, 0.0};

ThrustBody rocket = new ThrustBody(
	rocketInitialTrajectory,
	thrustKeys,
	angularThrustKeys,
	rocketLength,			// rocket length in meters
	exhaustVelocity,		// rocket exhaust velocity in m/s
	fuelBurnRate,			// rocket fuel burn rate in kg/s
)
```

## The actual calculation

Once we have created a rocket and the planets that are included in our simulation, we can begin calculating it's trajectory

```cs
for(int currentTime = 0; currentTime < thrustKeys.Length; current++){

	// print what iteration it is currently calculating for
	Console.WriteLine("Currently on iteration: " + currentTime);

	// calculate the next trajectory with a list of planets to take into consideration
	rocket.CalculateNext(planets);

	// print the trajectoryData
	rocket.CurrentTrajectoryData.PrintToConsole();
}
```
