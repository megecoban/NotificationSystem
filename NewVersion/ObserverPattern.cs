using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    public void NotifyCheck();
}

public interface ISubject
{
    public void Notify();
}

public abstract class ASubject : ISubject
{
    public IObserver observer;

    public void Notify()
    {
        observer.NotifyCheck();
    }
}