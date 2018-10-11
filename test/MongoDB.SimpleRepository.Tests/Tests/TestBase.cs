using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.SimpleRepository.Tests.Tests
{
    public abstract class TestBase
    {
        protected static int Rand()
        {
            return new Random().Next(0, 1000);
        }
    }
}
