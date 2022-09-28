// Whether to allow recursive Lock (No)
// SpinLock Policy (5000 Times -> Yield)
class Lock
{
    private const int EMPTY_FLAG = 0x00000000;  // 0000 0000 0000 0000 0000 0000 0000 0000
    private const int WRITE_MASK = 0x7FFF0000;  // 0111 1111 1111 1111 0000 0000 0000 0000
    private const int READ_MASK = 0x0000FFFF;   // 0000 0000 0000 0000 1111 1111 1111 1111
    private const int MAX_SPIN_COUNT = 5000;

    // bits : [Unused(1)] [WriteThreadId(15)] [ReadCount(16)]
    private int _flag = EMPTY_FLAG;

    public void WriteLock()
    {
        // When no one is acquiring WriteLock or ReadLock, they compete for ownership.
        int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;
        while (true)
        {
            for (int i = 0; i < MAX_SPIN_COUNT; i++)
            {
                // If you try and succeed, return
                if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG).Equals(EMPTY_FLAG))
                {
                    return;
                }
            }

            Thread.Yield();
        }
    }

    public void WriteUnlock()
    {
        Interlocked.Exchange(ref _flag, EMPTY_FLAG);
    }

    public void ReadLock()
    {
        // If no one is acquiring WriteLock, increase ReadCount by 1.
        while (true)
        {
            for (int i = 0; i < MAX_SPIN_COUNT; i++)
            {
                int expected = (_flag & READ_MASK);
                if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                {
                    return;
                }
            }

            Thread.Yield();
        }
    }

    public void ReadUnlock()
    {

    }
}