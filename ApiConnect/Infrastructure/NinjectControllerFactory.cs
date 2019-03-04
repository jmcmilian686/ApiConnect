using ApiConnect.Domain.Abstract;
using ApiConnect.Domain.Concrete;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ApiConnect.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;
        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null?null:(IController)ninjectKernel.Get(controllerType);
        }
        private void AddBindings()
        {
            // put bindings here
            ninjectKernel.Bind<IFieldsRepository>().To<EFFieldRepository>();
            ninjectKernel.Bind<IDataFieldRepository>().To<EFDataFieldRepository>();
            ninjectKernel.Bind<IRobotRepository>().To<EFRobotRepository>();
        }
    }
}