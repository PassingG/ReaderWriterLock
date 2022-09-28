internal class Program
{
    private static volatile int count = 0;
    private static Lock _lock = new Lock();

    private static void Main(string[] args)
    {
        Task t1 = new Task(delegate ()
        {
            for (int i = 0; i < 1000000; i++)
            {
                _lock.WriteLock();
                count++;
                _lock.WriteUnlock();
            }
        });

        Task t2 = new Task(delegate ()
        {
            for (int i = 0; i < 1000000; i++)
            {
                _lock.WriteLock();
                count--;
                _lock.WriteUnlock();
            }
        });

        t1.Start();
        t2.Start();

        Task.WaitAll(t1, t2);

        Console.WriteLine(count);
    }
}