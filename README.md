## PlayerLoopInjector

Simple utility to inject into the internal Unity PlayerLoop.

Currently contains injection points for the following points in the PlayerLoop:
- Initialization
- EarlyUpdate
- PreUpdate
- Update
- PostUpdate
- EndOfFrame
- FixedUpdate

## Installation

### Using PackageManager
You can install it via the Package Manager with the following url:  
`https://github.com/Refsa/PlayerLoopInjector.git#v0.1.0`

### Cloning Repository
You could also directly clone the repository into your projects `Assets/` folder or sub-module it.

## Example
Two ways to inject into the existing PlayerLoop exists, either via Injector.Global or Injector.Inject(...)

### Global Injection:
```cs
Injector.Global.OnPreUpdate += () => 
{
    // Do Stuff
}
```

### System Injection
Several interfaces exists to make this easier to deal with:
- IPlayerLoopInitialization
- IPlayerLoopEarlyUpdate
- IPlayerLoopPreUpdate
- IPlayerLoopUpdate
- IPlayerLoopPostUpdate
- IPlayerLoopFixedUpdate
- IPlayerLoopEndOfFrame
- IPlayerLoop

IPlayerLoop is just a collection of all the above, if you want to hook into all injection points.

Unless the object that inherits from these interfaces are based of UnityEngine.Object (MonoBehaviour, etc) you should manually remove it when destroyed. It keeps an internal reference to the owner object that will cause a dangling reference.

```cs
class ToInject : IPlayerLoopEarlyUpdate
{
    public void PlayerLoopEarlyUpdate()
    {
        // Do Stuff
    }
}

var instance = new ToInject();
Injector.Inject(instance);

// You should later remove it if the instance object is destroyed
Injector.Remove(instance);
```

Another option is to inject without the interface.
```cs
class SomeClass
{
    public void SomeMethod() {}
}

var instance = new SomeClass();
Injector.Inject(instance, LoopInjectionPoint.Update, instance.SomeMethod);
```