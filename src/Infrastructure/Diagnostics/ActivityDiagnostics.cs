using System.Diagnostics;
using System.Reflection;
using Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Diagnostics
{
    public static class ActivityDiagnostics
    {
        private static readonly ActivitySource activitySource;

        public const string InstanceIdKey = "InstanceId";

        public const string OperationNameKey = "OperationName";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
        static ActivityDiagnostics()
        {
            var assemblyName = Assembly.GetEntryAssembly()?.GetName();
            activitySource = assemblyName != null && assemblyName.Name != null && assemblyName.Version != null
                ? new ActivitySource(assemblyName.Name, assemblyName.Version!.ToString())
                : new ActivitySource(Constants.ApplicationAcronym);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
        public static void EnableActivityLogging(this IServiceCollection services)
        {
            var activityListener = new ActivityListener
            {
                ShouldListenTo = (ActivitySource source) => source == activitySource,
                Sample = delegate
                {
                    return ActivitySamplingResult.AllData;
                }
            };
            ActivitySource.AddActivityListener(activityListener);
            _ = services.AddSingleton(activityListener);
        }

        public static (bool IsNew, Activity? Instance) CreateActivityOrReuse(string operationName, string? instanceId = null)
        {
            var isNew = Activity.Current == null;

            var obj = isNew ? activitySource.CreateActivity(operationName, ActivityKind.Internal) : Activity.Current;

            if (obj != null)
            {
                _ = obj.SetBaggage(OperationNameKey, operationName);
                _ = obj.SetBaggage(InstanceIdKey, instanceId);
                _ = obj.Start();
            }

            return (isNew, obj);
        }
    }
}
