using UnityEngine;

namespace WastelandVehicleShooter
{
    /// <summary>
    /// Configures Unity project settings for optimal multiplayer performance
    /// </summary>
    public class ProjectSetup : MonoBehaviour
    {
        [Header("Physics Settings")]
        [SerializeField] private float fixedTimestep = 0.02f; // 50Hz
        [SerializeField] private float maximumAllowedTimestep = 0.1f;
        [SerializeField] private int defaultSolverIterations = 6;
        [SerializeField] private int defaultSolverVelocityIterations = 1;

        [Header("Quality Settings")]
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private int vSyncCount = 0; // Disable V-Sync

        private void Awake()
        {
            ConfigureProjectSettings();
        }

        /// <summary>
        /// Configures Unity project settings for multiplayer development
        /// </summary>
        private void ConfigureProjectSettings()
        {
            // Configure Physics Settings
            Time.fixedDeltaTime = fixedTimestep;
            Time.maximumDeltaTime = maximumAllowedTimestep;
            
            Physics.defaultSolverIterations = defaultSolverIterations;
            Physics.defaultSolverVelocityIterations = defaultSolverVelocityIterations;

            // Configure Quality Settings
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = vSyncCount;

            Debug.Log($"[ProjectSetup] Configured multiplayer settings:");
            Debug.Log($"  - Fixed Timestep: {fixedTimestep}s ({1f/fixedTimestep}Hz)");
            Debug.Log($"  - Max Timestep: {maximumAllowedTimestep}s");
            Debug.Log($"  - Solver Iterations: {defaultSolverIterations}");
            Debug.Log($"  - Target Frame Rate: {targetFrameRate} FPS");
            Debug.Log($"  - V-Sync: {(vSyncCount == 0 ? "Disabled" : "Enabled")}");
        }

        /// <summary>
        /// Validates current project settings
        /// </summary>
        [ContextMenu("Validate Settings")]
        public void ValidateSettings()
        {
            Debug.Log("=== Current Project Settings ===");
            Debug.Log($"Fixed Delta Time: {Time.fixedDeltaTime}s ({1f/Time.fixedDeltaTime}Hz)");
            Debug.Log($"Maximum Delta Time: {Time.maximumDeltaTime}s");
            Debug.Log($"Physics Solver Iterations: {Physics.defaultSolverIterations}");
            Debug.Log($"Physics Velocity Iterations: {Physics.defaultSolverVelocityIterations}");
            Debug.Log($"Target Frame Rate: {Application.targetFrameRate}");
            Debug.Log($"V-Sync Count: {QualitySettings.vSyncCount}");
        }
    }
}