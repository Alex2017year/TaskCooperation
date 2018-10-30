using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TaskCooperation
{
    class Program
    {

        static void ContinueWithDemo()
        {
            Task task1 = new Task(() =>
            {
                Console.WriteLine("Finished the first task");
            });

            Task task2 = task1.ContinueWith((tsk) =>
            {
                Console.WriteLine("Get the first task status: {0}", tsk.Status);
            });

            task1.Start();
            task2.Wait(); // 等待task2 执行完成。
            Console.WriteLine("Get the second task status: {0}", task2.Status);
        }

        static void ContinueWithDemo2()
        {
            Task.Run(() => 
            {
                Console.WriteLine("Finished the first task");
            }).ContinueWith((tsk) => 
            {
                Console.WriteLine("Get the first task status: {0}", tsk.Status);
            }).Wait();

        }


        static void Main(string[] args)
        {
            // ContinueWithDemo();
            // ContinueWithDemo2();
            // UseConditionalContinueWith();
            // UseParentAndChildren();
            // UseParentAndChildren2();
            UseWaitAll();


            Console.ReadKey();
        }

        static void UseConditionalContinueWith()
        {
            Task<int> task = Task.Run<int>(() => 
            {
                int value = new Random().Next(1, 1000);
                // throw new Exception("无效值");
                return value;
            });

            // 前一个任务完成时
            task.ContinueWith((prevTsk) => 
            {
                Console.WriteLine("前一个任务传来的值为：{0}", prevTsk.Result);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            // 前一个任务失败时
            task.ContinueWith((prevTsk) => 
            {
                Console.WriteLine("\n任务在执行时出现未捕获异常，其信息为：\n{0}", prevTsk.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);

            // 前一个任务被取消时
            task.ContinueWith((prevTsk) => 
            {
                Console.WriteLine("\n前一个任务被取消\n");
            }, TaskContinuationOptions.OnlyOnCanceled);


            try
            {
                task.Wait();
                Console.WriteLine("工作结束");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n使用try...catch捕获Wait()方法抛出的异常：\n{0}", ex);
            }

        }

        #region "一父多子类型的任务"
        /// <summary>
        /// 第一种方式，父任务中创建子任务，然后等待其完成
        /// </summary>
        static void UseParentAndChildren()
        {
            Task task = new Task(()=> 
            {
                Console.WriteLine("父任务开始执行....");
                Console.WriteLine("父任务启动两个子任务：");

                Task child1 = Task.Run(()=>
                {
                    Console.WriteLine("子任务1 开始启动");
                    Task.Delay(1000).Wait();
                    Console.WriteLine("子任务1 结束");
                });

                Task child2 = Task.Run(() =>
                {
                    Console.WriteLine("子任务2 开始启动");
                    Task.Delay(1000).Wait();
                    Console.WriteLine("子任务2 结束");
                });

                // 如果没有WaitAll（）,那么，父任务将在子任务之前结束
                // 可以试着注释掉以下这句，看看效果
                Task.WaitAll(child1, child2); // 等待子任务完成，父任务才结束
            });

            task.Start();
            // 等待整个任务树的完成
            task.Wait();
            Console.WriteLine("父任务完成了自己的工作，功成身退");
        }

        /// <summary>
        /// 方式二：不使用Task.Run()创建子任务，而是使用
        /// Task.Factory.StartNew()方法创建子任务，并
        /// 传给它一个TaskCreationOptions.AttachedToParent参数
        /// 从而无需在父任务中WaitAll()
        /// </summary>
        static void UseParentAndChildren2()
        {
            Task taskParent = Task.Factory.StartNew(()=>
            {
                Console.WriteLine("父任务开始……");
                //父任务完成的工作……
                Console.WriteLine("父任务启动了两个子任务");
                //创建后继子任务并自动启动

                Task child1 = Task.Factory.StartNew(()=> 
                {
                    Console.WriteLine("子任务一在行动……");
                    Task.Delay(1000).Wait();
                    Console.WriteLine("子任务一结束");
                }, TaskCreationOptions.AttachedToParent);

                Task child2 = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("子任务二在行动……");
                    Task.Delay(1000).Wait();
                    Console.WriteLine("子任务二结束");
                }, TaskCreationOptions.AttachedToParent);

            });

            // 等待整个任务树的完成
            taskParent.Wait();
            Console.WriteLine("父任务完成了自己的工作，功成身退。\n");
        }

        #endregion

        #region"使用WaitAll"
        static void UseWaitAll()
        {
            Console.WriteLine("启动三个并行任务....\n");
            var t1 = Task.Run(() => { DoSomeVeryImportantWork(1, 3000); });
            var t2 = Task.Run(() => { DoSomeVeryImportantWork(2, 1000); });
            var t3 = Task.Run(() => { DoSomeVeryImportantWork(3, 300); });

            Task.WaitAll(new Task[] { t1, t2, t3 });
            Console.WriteLine("\n所有工作都执行完了。");
        }
        static void DoSomeVeryImportantWork(int id, int sleepTime)
        {
            Console.WriteLine("任务{0}正在执行……", id);
            Thread.Sleep(sleepTime);
            Console.WriteLine("任务{0}执行结束。", id);
        }
        #endregion

        #region "使用ContinueWhenAll"




    }
}
