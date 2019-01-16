namespace AntRunner.Interface
{
    /// <summary>
    /// Static class of constant values used when assigning damage to an ant.
    /// </summary>
    public static class DamageValues
    {
        /// <summary>
        /// Damage applied when an ant runs into an object.
        /// </summary>
        public const int Collision = 5;

        /// <summary>
        /// Damage applied when an ant is run into or rammed by another ant.
        /// </summary>
        public const int Impact = 10;

        /// <summary>
        /// Damage applied when an ant is shot with a laser.
        /// </summary>
        public const int Shot = 20;

        /// <summary>
        /// Damage applied when an ant steps on a bomb.
        /// </summary>
        public const int Bomb = 30;
    }
}
