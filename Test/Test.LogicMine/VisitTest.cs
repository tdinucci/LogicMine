using System;
using System.Linq;
using System.Runtime.InteropServices;
using LogicMine;
using Xunit;

namespace Test.LogicMine
{
    public class VisitTest
    {
        [Fact]
        public void Construct()
        {
            RunConstructTest(Guid.NewGuid().ToString(), VisitDirection.Down);
            RunConstructTest(Guid.NewGuid().ToString(), VisitDirection.Up);
        }

        [Fact]
        public void Construct_BadArgs()
        {
            Assert.Throws<ArgumentException>(() => new Visit(null, VisitDirection.Down));
            Assert.Throws<ArgumentException>(() => new Visit(string.Empty, VisitDirection.Down));
            Assert.Throws<ArgumentException>(() => new Visit("     \t \n \r", VisitDirection.Down));
        }
        
        [Fact]
        public void ModifyWritable()
        {
            var exMessage = Guid.NewGuid().ToString();
            var visit = new Visit(Guid.NewGuid().ToString(), VisitDirection.Down)
            {
                Exception = new InvalidOleVariantTypeException(exMessage),
                Duration = TimeSpan.FromHours(1.5)
            };

            Assert.Equal(typeof(InvalidOleVariantTypeException), visit.Exception.GetType());
            Assert.Equal(exMessage, visit.Exception.Message);
            Assert.Equal(TimeSpan.FromHours(1.5), visit.Duration);
        }

        [Fact]
        public void LogMessages()
        {
            const int logCount = 50;
            var visit = new Visit(Guid.NewGuid().ToString(), VisitDirection.Down);
            for (var i = 0; i < logCount; i++)
                visit.Log(i.ToString());

            Assert.Empty(visit.LogWarnings);
            Assert.Empty(visit.LogErrors);
            Assert.True(visit.LogMessages.Count() == logCount);

            var index = 0;
            foreach (var logMessage in visit.LogMessages)
            {
                var timeString = logMessage.Substring(1, logMessage.IndexOf(']') - 1);
                var time = DateTime.Parse(timeString);

                Assert.True(time < DateTime.Now && time > DateTime.Now.AddSeconds(-1));
                Assert.Equal(index.ToString(), logMessage.Substring(logMessage.IndexOf(']') + 2));
                index++;
            }
        }

        [Fact]
        public void LogWarnings()
        {
            const int logCount = 50;
            var visit = new Visit(Guid.NewGuid().ToString(), VisitDirection.Down);
            for (var i = 0; i < logCount; i++)
                visit.LogWarning(i.ToString());

            Assert.Empty(visit.LogMessages);
            Assert.Empty(visit.LogErrors);
            Assert.True(visit.LogWarnings.Count() == logCount);

            var index = 0;
            foreach (var warning in visit.LogWarnings)
            {
                var timeString = warning.Substring(1, warning.IndexOf(']') - 1);
                var time = DateTime.Parse(timeString);

                Assert.True(time < DateTime.Now && time > DateTime.Now.AddSeconds(-1));
                Assert.Equal(index.ToString(), warning.Substring(warning.IndexOf(']') + 2));
                index++;
            }
        }

        [Fact]
        public void LogErrors()
        {
            const int logCount = 50;
            var visit = new Visit(Guid.NewGuid().ToString(), VisitDirection.Down);
            for (var i = 0; i < logCount; i++)
                visit.LogError(i.ToString());

            Assert.Empty(visit.LogMessages);
            Assert.Empty(visit.LogWarnings);
            Assert.True(visit.LogErrors.Count() == logCount);

            var index = 0;
            foreach (var error in visit.LogErrors)
            {
                var timeString = error.Substring(1, error.IndexOf(']') - 1);
                var time = DateTime.Parse(timeString);

                Assert.True(time < DateTime.Now && time > DateTime.Now.AddSeconds(-1));
                Assert.Equal(index.ToString(), error.Substring(error.IndexOf(']') + 2));
                index++;
            }
        }

        [Fact]
        public void LogException()
        {
            const int logCount = 5;
            var visit = new Visit(Guid.NewGuid().ToString(), VisitDirection.Down);
            for (var i = 0; i < logCount; i++)
            {
                var exception = new InvalidOperationException($"Bad thing {i}");
                visit.LogError(exception);
            }

            Assert.Empty(visit.LogMessages);
            Assert.Empty(visit.LogWarnings);
            Assert.True(visit.LogErrors.Count() == logCount);

            var index = 0;
            foreach (var error in visit.LogErrors)
            {
                var timeString = error.Substring(1, error.IndexOf(']') - 1);
                var time = DateTime.Parse(timeString);

                Assert.True(time < DateTime.Now && time > DateTime.Now.AddSeconds(-1));
                index++;
            }
        }
        
        private void RunConstructTest(string description, VisitDirection direction)
        {
            var visit = new Visit(description, direction);
            Assert.Equal(description, visit.Description);
            Assert.Equal(direction, visit.Direction);
            Assert.True(visit.StartedAt < DateTime.Now && visit.StartedAt > DateTime.Now.AddSeconds(-1));
            Assert.Null(visit.Duration);
            Assert.True(visit.LogMessages != null && !visit.LogMessages.Any());
            Assert.Null(visit.Exception);
        }
    }
}