using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

public class Loom : MonoBehaviour
{
    /// <summary>
    /// 最大线程数
    /// </summary>
    public static int maxThreads = 8;
    /// <summary>
    /// 当前线程编号
    /// </summary>
    static int numThreads;
    /// <summary>
    /// 字段，loom 类型字段 _current
    /// </summary>
    private static Loom _current;
    /// <summary>
    /// ？？？？
    /// </summary>
    private int _count;
    /// <summary>
    /// 属性，_current的属性
    /// </summary>
    public static Loom Current
    {
        get
        {
            Initialize();
            return _current;
        }
    }

    void Awake()
    {
        _current = this;
        initialized = true;
    }
    /// <summary>
    /// bool 值 已经初始化了
    /// </summary>
    static bool initialized;
    /// <summary>
    /// 初始化，生成loom脚本
    /// </summary>
    static void Initialize()
    {
        //如果还没有初始化
        if (!initialized)
        {
            //如果游戏没有在运行
            if (!Application.isPlaying)
                return;
            initialized = true;
            var g = new GameObject("Loom");
            _current = g.AddComponent<Loom>();
        }

    }
    /// <summary>
    /// 集合，action 类型的集合，是一个委托
    /// </summary>
    private List<Action> _actions = new List<Action>();
    /// <summary>
    /// 结构，延时队列方法，一个延时时间，一个委托方法
    /// </summary>
    public struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }
    /// <summary>
    /// 集合，需要延时队列方法的集合
    /// </summary>
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();
    /// <summary>
    /// 集合，已经延时到达的延时队列的方法集合
    /// </summary>
    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();
    /// <summary>
    /// 在主线程调用一个方法的时候，直接调用已有方法，延时0秒
    /// </summary>
    /// <param name="action"></param>
    public static void QueueOnMainThread(Action action)
    {
        QueueOnMainThread(action, 0f);
    }
    /// <summary>
    /// 真正的在主线程调用一个方法，可以传参数：方法，延时时间
    /// </summary>
    /// <param name="action"></param>
    /// <param name="time"></param>
    public static void QueueOnMainThread(Action action, float time)
    {
        if (time != 0)
        {
            //如果存在延时时间，把延时队列锁住，等当前线程添加完新的数据以后，后面的线程才能继续添加
            lock (Current._delayed)
            {
                Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }
        }
        else
        {   //如果不存在延时时间，把action 队列锁住，等当前线程添加完新的数据以后，后面的线程才能继续添加
            lock (Current._actions)
            {
                Current._actions.Add(action);
            }
        }
    }
    /// <summary>
    /// 返回一个线程，异步运行委托 a
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Thread RunAsync(Action a)
    {
        //如果没有初始化，则初始化
        Initialize();
        //如果当前线程编号大于最大线程数，则当前线程休眠1毫秒？？？
        while (numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }
        //当次方法被调用的时候，线程编号增加
        Interlocked.Increment(ref numThreads);
        //线程池，启动工作者进程，启动一个委托，a 是runaction的参数
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }
    /// <summary>
    /// 将线程池中传进来的action参数，运行
    /// </summary>
    /// <param name="action"></param>
    private static void RunAction(object action)
    {
        try
        {   //运行委托
            ((Action)action)();
        }
        catch
        {
        }
        finally
        { //当次方法调用完成，线程编号减少
            Interlocked.Decrement(ref numThreads);
        }

    }

    /// <summary>
    /// 当场景，游戏结束的时候，如果Loom对象就是当前对象，就清空
    /// </summary>
    void OnDisable()
    {
        if (_current == this)
        {

            _current = null;
        }
    }



    // Use this for initialization
    void Start()
    {

    }
    /// <summary>
    /// 集合，当前委托集合
    /// </summary>
    List<Action> _currentActions = new List<Action>();

    // Update is called once per frame
    void Update()
    {
        //锁住，不存在延时时间的，委托方法集合
        lock (_actions)
        {
            //将当前委托集合清空
            _currentActions.Clear();
            //将不存在委托时间的委托集合，全部添加到当前委托集合中
            _currentActions.AddRange(_actions);
            //将不存在委托时间的委托集合清空
            _actions.Clear();
        }
        //将当前委托集合中的所有方法全部运行
        foreach (var a in _currentActions)
        {
            a();
        }
        //锁住，有延时时间的方法集合
        lock (_delayed)
        {   //将已经运行过的具有延时的委托集合清空
            _currentDelayed.Clear();
            //将需要延时的方法，加入到当前已经延时完的委托集合中  12.00.00+00.00.05 < 12.00.08
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            //每一个已经延时到时间的方法，都从需要延时集合中移除
            foreach (var item in _currentDelayed)
                _delayed.Remove(item);
        }
        //每一个在 当前延时已经到时间的委托，全部运行
        foreach (var delayed in _currentDelayed)
        {
            delayed.action();
        }



    }
}