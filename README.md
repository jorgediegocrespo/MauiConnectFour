# Connect 4 with dotNet MAUI
Full connect 4 game app, developed with **dotNet MAUI.**

Here are some GIFs where you can see the design and behavior of the app. And I also detail the nugets that I have used and the reasons that have led me to use them.  

## Design and behavior
![Playing connect four](/images/ConnectFourPlaying.gif)  
![Player won](/images/ConnectFourWinner.gif)

## Nuggets  

-**SkiaSharp.Views.Maui.Controls:** I have used it to draw the board, since I couldn't think of any other way to paint the outside of a circumference, keeping its interior transparent, so that the pieces can be seen falling inside the board.  
-**ReactiveUI.Maui:** Obviously the app could have been developed without this nuget, but I always include it in my projects because I feel so comfortable working with it. I think it simplifies development a lot without penalizing the performance of my applications.  
-**ReactiveUI.Fody:** I use it to save some code when creating properties in my ViewModels.
