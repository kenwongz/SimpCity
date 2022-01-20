using System;
using System.Diagnostics.CodeAnalysis;

namespace SimpCity {
    /// <summary>
    /// Singleton wrapper for SimpCity random.
    /// </summary>
    [ExcludeFromCodeCoverage]
    class ScRandom : Random {
        private static ScRandom instance;
        public static ScRandom GetInstance() {
            if (instance != null) {
                return instance;
            }
            instance = new ScRandom();
            return instance;
        }
    }
}
