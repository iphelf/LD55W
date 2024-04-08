using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Roulette.Scripts.General
{
    public static class Dummy
    {
        public static async Awaitable NothingButAwaitable()
        {
            await Task.CompletedTask;
        }

        public static void PerformQuickTask(string description,
            [CallerMemberName] string memberName = "")
        {
            Debug.Log($"Dummy quick task: \"{description}\" from {memberName}");
        }

        public static async Awaitable PerformTask(string description,
            [CallerMemberName] string memberName = "")
        {
            await Awaitable.WaitForSecondsAsync(0.5f);
            Debug.Log($"Dummy task: \"{description}\" from {memberName}");
            await Awaitable.WaitForSecondsAsync(0.5f);
        }

        public static bool CheckCondition(string description, bool placeholder = true,
            [CallerMemberName] string memberName = "")
        {
            Debug.Log($"Dummy condition: \"{description}\" from {memberName}");
            return true;
        }
    }
}