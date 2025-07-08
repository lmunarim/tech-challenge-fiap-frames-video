using fiap.Application;
using fiap.Repositories;
using fiap.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace fiap.Tests
{
    public class BuilderTests
    {
         
        

        [Fact]
        public void AddModule_Test()
        {
            var _serviceCollection = new ServiceCollection();

            _serviceCollection.AddApplicationModule();
            _serviceCollection.AddServicesModule();
            _serviceCollection.AddRepositoriesModule();

            var _serviceProvider = _serviceCollection.BuildServiceProvider();


            Assert.NotNull(_serviceProvider);

        }
    }
}
