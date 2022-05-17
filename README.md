# Unity-GameScripts
This is the repository for scrips I have made for a project game. You are free to use these for your games. I focused on simplicity for most of these, meaning less dependencies.

This movement script includes - Walking, Running(FOV-Movement Included), Jumping, Crouching, Sliding, and Water-Movement (Moisty).

Everything is handled in the update script. You will also see that functions in the update script are contained in an "if" statement. Those functions return a Boolean based off of the check they are doing. Those checks are used in the "if" statements are used to turn on and off functions/movement based on those checks.

The required component's for this is a Camera that is a child of the Player(The gameObject this script is on) and "Moisty" which mimics the physics of buoyancy in water. What isn't required is the "GrassWalking" which can be commented out.

"GrassWalking" is an object that contains a AudioSource that has a walking sound attached to it. It will be turned on or off and change pitch based on if it is walking or running.

Also, the tags for the ground a water are "Ground" and "Water" respectively.

Note : I tried to make these script as readable as possible so that other people can add and change it. I would also like feedback for this as to get better at coding.
