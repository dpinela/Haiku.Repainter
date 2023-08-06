# Repainter

This is a mod for Haiku, the Robot that changes the colour palettes
of each area to randomly-generated ones. A new palette can be applied
at the start of the game, or at any moment while in-game; once set,
it is stored with the save file.

## Configuration

- *Seed*: determines which palette will be chosen. If left empty, a random* seed will be used each time.
- *Apply Palette on Game Start*: causes a palette to be set when starting a new file, as if you pressed "Apply Palette".
- *Disable in Creator Rooms*: disables the colorization while in the Neutron, Proton, Electron, and Virus rooms.
- *Apply Palette*: sets and applies a palette to the current save file, based on the seed.

## Known issues

You may experience frame rate drops in more graphically-intensive
rooms, particularly in the Creator rooms. If so, try the
*Disable in Creator Rooms* option to mitigate the issue.