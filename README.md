# Smiling Mobile

Smiling Mobile is a 3D physics-based bike platformer inspired by chaotic stunt games. The goal is simple: ride from start to finish through hazard-filled levels without wiping out.

## Genre

Driving / Obstacle Platformer (3D)

## Core Gameplay Loop

1. Accelerate through ramps, gaps, and hazards.
2. Keep balance using precise steering and braking.
3. Recover from rough landings and avoid crashing.
4. Reach the goal to complete the level.

## Features

- Physics-driven bicycle movement and collision response
- Obstacle-heavy level design (spikes, drops, moving hazards)
- Crash and ragdoll behavior for high-impact failures
- Respawn system for fast retries
- Collectible pickups that can contribute to score

## Controls (Current Build)

- `W/S` or `Up/Down`: accelerate / reverse
- `A/D` or `Left/Right`: tilt and steer
- `Space`: jump
- `Left Shift`: brake
- `G`: force ragdoll (debug/manual crash)
- `R`: respawn after a crash

## Win and Lose Conditions

### Win

- Reach the level goal.

### Lose

- Hit major hazards (for example spikes/traps).
- Fall off the course.
- Crash from extreme tilt or hard impact.

## UI Direction

- Title screen with Start and Settings
- In-game HUD for score and optional timer
- End screens for level clear and game over states

## Tech Stack

- Unity (3D)
- RayznGames Bicycle System
- Custom level geometry, hazards, and gameplay scripting

## Project Status

This project is currently in active development for Intro to Game Development (Spring 2026).
