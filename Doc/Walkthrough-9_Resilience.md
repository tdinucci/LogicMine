#### Walkthrough 9 - Resilience & Transient Faults
If you are developing "cloud" based systems then you'll be aware that transient errors can happen, i.e. you attempt to connect to a database service but for one reason or another you can't.  LogicMine itself does not provide any real logic to help you deal with these types of error however it does give you the ability to define your own fault handling logic and or to use another library specifically designed for the purpose.

In this walkthrough we'll demonstrate using [Polly](https://github.com/App-vNext/Polly) with LogicMine.  It won't go into great detail because the Polly documentation already does a good job of this.  Here we just want to see how Polly can be added to a service.

#### 1. Open your Database2 walkthrough project
*N.B. The code for these walkthroughs is included in the source respository, as a project per walkthrough.  The code within this walkthrough is taken from these projects and the namespaces will be slightly different to yours if you're following along with your own project.*

