using System;

namespace SimpCity {
    /// <summary>
    /// Singleton wrapper for SimpCity random.
    /// </summary>
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
