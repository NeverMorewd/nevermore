using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.dynamicproxy
{
    public class RealServiceProxy : RealProxy
    {
        private readonly Type m_ServiceType;
        private const int MAX_RETRY_TIMES = 5;

        public RealServiceProxy(Type serviceType)
            : base(serviceType)
        {
            m_ServiceType = serviceType;
        }

        static RealServiceProxy()
        {
        }

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage methodMessage = new MethodCallMessageWrapper((IMethodCallMessage)msg);

            var methodInfo = m_ServiceType.GetMethod(methodMessage.MethodName, GetArgTypes(methodMessage.Args));
            object objReturnValue = null;

            if (methodInfo == null)
            {
                if (methodMessage.MethodName.Equals("GetType") && (methodMessage.ArgCount == 0))
                {
                    objReturnValue = m_ServiceType;
                    return new ReturnMessage(objReturnValue, methodMessage.Args, methodMessage.ArgCount,
                        methodMessage.LogicalCallContext, methodMessage);
                }
                else
                {
                    return new ReturnMessage(objReturnValue, methodMessage.Args, methodMessage.ArgCount,
                        methodMessage.LogicalCallContext, methodMessage);
                }
            }

            if (methodMessage.MethodName.Equals("HeartBeatTesting"))
            {
                objReturnValue = HeartBeat(methodMessage.Args.First() as string);

                return new ReturnMessage(objReturnValue, methodMessage.Args, methodMessage.ArgCount,
                    methodMessage.LogicalCallContext, methodMessage);
            }

            // Prepare request.
            // 1. extract class and method information
            object[] copiedArgs = Array.CreateInstance(typeof(object), methodMessage.Args.Length) as object[];
            methodMessage.Args.CopyTo(copiedArgs, 0);

            var methodName = methodInfo.Name;
            var requestResponseAttribute = methodInfo.GetCustomAttributes(false)
                ?.Where(att => att is RailRequestResponseAttribute).Cast<RailRequestResponseAttribute>()
                .FirstOrDefault();

            if (requestResponseAttribute == null)
                throw new InvalidOperationException("Method should be tagged with RailRequestResponseAttribute!");

            if (!string.IsNullOrEmpty(requestResponseAttribute.MethodName))
                methodName = requestResponseAttribute.MethodName;

            var serverServiceMethodInfo = GetServiceMethodInfo(requestResponseAttribute.ServiceType, methodName,
                requestResponseAttribute.RequestType);

            if (serverServiceMethodInfo == null)
                throw new InvalidOperationException(
                    $"Method doesn't exist in the calling facade interface! Method name:{methodName}, Request type:{requestResponseAttribute.RequestType.ToString()}");

            var requestType = requestResponseAttribute.RequestType;
            Type serviceType = requestResponseAttribute.ServiceType;

            // 2. generate request object.
            BaseRequest request = GetRequest(requestType, copiedArgs);

            BaseResponse response = DoRequest(methodName, serverServiceMethodInfo, serviceType, request, MAX_RETRY_TIMES);

            // 3. generate resonse object.
            if (response != null)
            {
                objReturnValue = GetResult(response);

                if (response!=null)
                {
                    if (objReturnValue != null)
                    {
                    }
                }
            }

            // Create the return message (ReturnMessage)
            return new ReturnMessage(objReturnValue, methodMessage.Args, methodMessage.ArgCount,
                methodMessage.LogicalCallContext, methodMessage);
        }

        private BaseResponse DoRequest(string methodName, MethodInfo serverServiceMethodInfo, Type serviceType, BaseRequest request, int leftRetryTimes)
        {
            // Prepare service proxy intance
            // 1. request url.
            var currentEndPoint = LoadBalancer.Instance.GetAnEndPoint();
            if (currentEndPoint == null || !currentEndPoint.IsActive)
            {
                throw new ClientException().AddErrorCode(FrameworkRule.FENoActiveEndPoint);
            }

            var serverAddress = currentEndPoint.Address;
            string requestUri = string.Empty;

            var uriAttributes = ReflectionUtil.GetAttributes<ServiceUriAttribute>(serviceType, false);
            if (uriAttributes.Length > 0)
                requestUri = serverAddress + uriAttributes[0].RequestUri;

            if (string.IsNullOrWhiteSpace(requestUri))
            {
                throw new InvalidOperationException("Application facade URI does not configed.");
            }

            // 2. create proxy instance
            object serviceInstance = GetProxyImpl(serviceType, requestUri);

            // Perform request
            BaseResponse response = null;
            try
            {
                response = (BaseResponse)serverServiceMethodInfo.Invoke(serviceInstance, new[] { request });
            }
            catch (TargetInvocationException)
            {
            }

            return response;
        }

        private static object GetProxyImpl(Type serviceType, string requestUri)
        {
            CHessianProxyFactory factory = ApplicationManager.Instance.GetService<CHessianProxyFactory>();
            return factory.Create(serviceType, requestUri);
        }

        /// <summary>
        /// Returns array with types of the instance from 
        /// the argument array
        /// </summary>
        /// <param name="arrArgs">Any array</param>
        /// <returns>Array with types of the instance from 
        /// the argument array</returns>
        public static Type[] GetArgTypes(object[] arrArgs)
        {
            if (null == arrArgs)
            {
                return new Type[0];
            }

            Type[] result = new Type[arrArgs.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                if (arrArgs[i] == null)
                {
                    result[i] = null;
                }
                else
                {
                    result[i] = arrArgs[i].GetType();
                }
            }

            return result;
        }

        private static MethodInfo GetServiceMethodInfo(Type serviceType, string methodName, Type requestType)
        {
            var method = serviceType.GetMethod(methodName, new Type[] { requestType });

            return method;
            // verify the method info by input and output.
        }

        /// <summary>
        /// Do interface transfermation from programming contract to service contract.
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static BaseRequest GetRequest(Type requestType, object[] args)
        {
            var request = (BaseRequest)Activator.CreateInstance(requestType);
            string terminalCode = string.Empty;
            if (ApplicationManager.Instance.TryGetService<ITerminalContext>(out var context))
            {
                terminalCode = context.TerminalCode;
            }

            request.RequestProperties =
                new RequestProperties()
                {
                    TerminalCode = terminalCode
                };

            request.Initialize(args);

            return request;
        }

        public static object GetResult(BaseResponse response)
        {
            BusinessRuleExceptions exs = null;
            if (response.Acknowledgement.Status == Acknowledgement.STATUS_NONOK)
            {
                foreach (Reply reply in response.Acknowledgement.Replys)
                {
                    var errorCode = reply.ErrorCode ?? string.Empty;
                    var errorField = reply.ErrorField ?? string.Empty;
                    var errorLevel = reply.Level ?? BusinessRuleException.Level.Error.ToString();
                    // Add BusinessRuleException for all known errors.

                    BusinessRuleException error =
                        BusinessRuleException.CreateBusinessRuleException(reply.DefaultMessage, errorField);
                    if (Enum.TryParse<BusinessRuleException.Level>(errorLevel, out var exceptionLevel))
                    {
                        error.ExceptionLevel = exceptionLevel;
                    }

                    error.AddErrorCode(errorCode);
                    if (exs == null)
                        exs = new BusinessRuleExceptions(reply.DefaultMessage);
                    exs.AddErrorCode(errorCode);

                    // Add error data.
                    int idx = 0;
                    reply.CodeValues?.Execute(param => error.AddData((idx++).ToString(), param));

                    exs.AddBusinessRuleException(error);

                    RLog.Error(reply.DefaultMessage);
                }

                exs?.ThrowIfAny();
            }

            return response.InternalGetResult();
        }

        private static bool HeartBeat(string endPointAddress)
        {
            // WangBo said he will create a heartbeat later. temporarily, we can use login.
            string heartBeatMethod = "heartBeat";

            // Prepare service proxy intance
            // If the service proxy doesn't registed. Create service proxy instance and call.
            var serviceType = typeof(IRailSecurityInboundFacade);

            string requestUri = endPointAddress +
                                "/tos.rail.webservices.gateway/hessian/tos.rail.inbound.facade.RailSecurityInboundFacade";

            var factory = new CHessianProxyFactory() { IsOverloadEnabled = true };
            object serviceInstance = factory.Create(serviceType, requestUri);

            try
            {
                var mi = serviceType.GetMethod(heartBeatMethod);

                if (mi != null)
                {
                    mi.Invoke(serviceInstance, new object[] { });
                    return true;
                }
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException.Message.Contains("(401)"))
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }

    public class RailRequestResponseAttribute : Attribute
    {
        public RailRequestResponseAttribute(Type serviceType, Type requestType, Type responseType, string methodName = "")
        {
            ServiceType = serviceType;
            RequestType = requestType;
            ResponseType = responseType;
            MethodName = methodName;
        }

        public string MethodName { get; }
        public Type ServiceType { get; }
        public Type RequestType { get; }
        public Type ResponseType { get; }
    }
    public class BaseRequest
    { }
    public class BaseResponse
    { }
}
