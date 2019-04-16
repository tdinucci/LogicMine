## Walkthrough 6 - Inter-Station Communication
Please make sure you have followed the second walkthrough (Stations) as this one follows on from it.

In a perfect world all of our stations would be completely independant, i.e. they wouldn't rely on other stations doing anything special before they're visited.  If we have this then our stations become coupled (to some degree) and if they're coupled then a change to one station could end up causing a problem in another.  We don't want this but we have to live in the real world.

We have some options if we find we want to make one station dependant on the output of another:

  1) We merge the stations.  This may, or may not make the dangers of coupling worse.  Here we've said these two (what we previously considered) distinct operations are going to become one and there is a very real risk that they will be/become practically inseperable later.
  
  2) We augment our requests with *internal* properties which allows for one station to set a property and another to read it.
  
  3) We write to the IRequest.Options dictionary and each station/terminal can inspect and modify this.  This isn't too disimilar to option 2) and it also suffers from lack of type safety since all values in the dictionary are just of type *object*.
  
In this walkthrough we'll try out option 2).

#### 1. Open your Stations walkthrough project
That's all.

*N.B. The code for these walkthroughs is included in the source respository, as a project per walkthrough.  The code within this walkthrough is taken from these projects and the namespaces will be slightly different to yours if you're following along with your own project.*

#### 2. Delete the current stations
Delete the files; *ReverseResponseStation.cs*, *MakeNameUppercaseStation.cs* and *SurroundNameWithStarsStation.cs*.

Remove the deleted stations from *HelloShaftRegistrar*.
