using System;

namespace Utilities
{
    public static class Constants
    {
        // Lap
        public const int DEFAULT_LAP_COUNT = 3;
        public const int MIN_LAP_COUNT = 1;
        public const int MAX_LAP_COUNT = 10;

        // UI Text
        public const string SOLO_RACE_WIN = "YOU WIN!";
        public const string SOLO_RACE_LOSE = "YOU LOSE!";
        public const string SOLO_TIMED_WIN = "AWARD: ";
        public const string SOLO_TIMED_LOSE = "FAILED!";
        public const string VERSUS_WINNER_TEXT = "WINNER: ";
        public const string BEST_LAP_TEXT = "BEST LAP TIME: ";
        public const string RACE_FINISHED = "FINISHED";

        public const float MAX_IDLE_SPEED = 0.5f;
        public const float MIN_THROTTLE = 0.1f;
        public const float MIN_SPEED_FOR_TURN = 5f;
        public const float STEERING_RIGHT_ANGLE = 90f;
        public const float STEERING_LEFT_ANGLE = -90f;
        public const float STEERING_ANIM_LERP_SPEED = 10f;
        public const float EMISSION_MOVE_TOWARDS_RATE = 10f;
        public const float IDLE_EXHAUST_RATE = 1f;
        public const float IDLE_DRIFT_RATE = 0f;

        // Collision
        public const float COLLISION_EFFECT_COOLDOWN_TIME = 0.3f;
        public const float COLLISION_VOLUME_COEFFICIENT = 0.5f;
        public const float COLLISION_DURATION_COEFFICIENT = 0.5f;
        public const float COLLISION_INTENSITY_COEFFICIENT = 0.8f;

        // Terrain
        public const float ROAD_TERRAIN_FACTOR = 0f;
        public const float GRASS_TERRAIN_FACTOR = 0.6f;
        public const float DIRT_TERRAIN_FACTOR = 0.8f;
        public const float GRAVEL_TERRAIN_FACTOR = 1f;
        public const float OFFROAD_VOLUME_COEFFICIENT = 0.75f;

        // Camera
        public const float DYNAMIC_CAMERA_LOOK_AHEAD_TIME = 0.5f;
        public const float DYNAMIC_CAMERA_LOOK_AHEAD_SMOOTHING = 5f;
        public const float MIN_ORTHOGRAPHIC_CAMERA_SIZE = 70f;
        public const float MAX_ORTHOGRAPHIC_CAMERA_SIZE = 114f;
        public const float ZOOM_FACTOR_CONSTANT = 0.5f;

        // Audio
        public const string MASTER_AUDIO_MIXER = "Master";
        public const string MUSIC_AUDIO_MIXER = "Music";
        public const string UI_AUDIO_MIXER = "UI";
        public const string EFFECTS_AUDIO_MIXER = "Effects";

        public const float AUDIO_MUSIC_FADE_IN_TIME = 0.5f;
        public const float AUDIO_EFFECTS_FADE_IN_TIME = 0.25f;
        public const float AUDIO_EFFECTS_FADE_OUT_TIME = 0.5f;

        // Engine Audio
        public const float DYNAMIC_VOLUME_LERP_SPEED = 5f;
        public const float DYNAMIC_PITCH_LERP_SPEED = 5f;
        public const float THROTTLE_LERP_SPEED = 5f;
        public const float RPM_LERP_SPEED = 0.15f;
        public const float RPM_STEERING_FACTOR_THRESHOLD = 0.65f;


        public const float ACCEL_LOW_VOLUME_COEFFICIENT = 0.7f;
        public const float ACCEL_HIGH_VOLUME_COEFFICIENT = 0.5f;
        public const float DECEL_LOW_VOLUME_COEFFICIENT = 0.8f;
        public const float DECEL_HIGH_VOLUME_COEFFICIENT = 0.6f;

        public const float ACCEL_LOW_PITCH_COEFFICIENT = 0.8f;
        public const float ACCEL_HIGH_PITCH_COEFFICIENT = 0.8f;
        public const float DECEL_LOW_PITCH_COEFFICIENT = 0.8f;
        public const float DECEL_HIGH_PITCH_COEFFICIENT = 0.6f;

        // AI
        public const float AI_SPLINE_MIN_LOOK_AHEAD = 0.01f;
        public const float AI_SPLINE_MAX_LOOK_AHEAD = 0.02f;

        public const float AI_EASY_MAX_STEERING_ANGLE = 60f;
        public const float AI_EASY_STEER_SMOOTHING = 35f;
        public const float AI_EASY_MAX_THROTTLE = 0.9f;
        public const float AI_EASY_MIN_THROTTLE = 0.5f;
        public const float AI_EASY_MIN_BRAKE_SPEED_FACTOR = 0.6f;
        public const float AI_EASY_BRAKE_ANGLE = 25f;
        public const float AI_EASY_BRAKE_TIME = 0.4f;
        public const float AI_EASY_STUCK_DETECTION_TIME = 1.5f;

        public const float AI_HARD_MAX_STEERING_ANGLE = 50f;
        public const float AI_HARD_STEER_SMOOTHING = 50f;
        public const float AI_HARD_MAX_THROTTLE = 1f;
        public const float AI_HARD_MIN_THROTTLE = 0.8f;
        public const float AI_HARD_MIN_BRAKE_SPEED_FACTOR = 0.75f;
        public const float AI_HARD_BRAKE_ANGLE = 40f;
        public const float AI_HARD_BRAKE_TIME = 0.2f;
        public const float AI_HARD_STUCK_DETECTION_TIME = 0.75f;

        public const float AI_UNSTUCK_DETECTION_TIME = 0.5f;
        public const float AI_MAX_STUCK_DETECTION_SPEED = 2f;
        public const float AI_MAX_STUCK_DETECTION_DISTANCE = 5.5f;
        public const float AI_MAX_CLEAR_DETECTION_DISTANCE = 8f;

        public static string FormatTime(float lapTime)
        {
            string formattedTime = TimeSpan.FromSeconds(lapTime).ToString(@"mm\:ss\.ff");

            return formattedTime;
        }
    }

    public enum GameState
    {
        Menu,
        Loading,
        Playing,
        Paused,
        GameOver
    }

    public enum MenuScreenType
    {
        Home,
        Selection,
        Vehicle,
        Settings
    }

    public enum SettingScreenType
    {
        Game,
        Audio,
        Controls,
        InputPopup
    }

    public enum GameMode
    {
        Race,
        Timed
    }

    public enum Medal
    {
        None,
        Failed,
        Bronze,
        Silver,
        Gold
    }

    public enum VehicleState
    {
        Idle,
        Accelerating,
        Decelerating,
        Braking,
        Reversing
    }

    public enum EffectRate
    {
        None,
        Low,
        High
    }

    public enum PresetVehicle : int
    {
        AllRounder = 0,
        Drifter = 1,
        Muscle = 2,
        Racer = 3
    }

    public enum TerrainType : int
    {
        Road = 0,
        Grass = 1,
        Dirt = 2,
        Gravel = 3,
    }

    public enum CameraMode
    {
        Fixed,
        Dynamic
    }

    public enum ScreenShake
    {
        Off,
        Low,
        High
    }

    public enum Difficulty
    {
        Easy,
        Hard
    }

    public enum ControlInput
    {
        Throttle,
        Reverse,
        Left,
        Right,
        Handbrake
    }

    public enum BridgeTriggerType
    {
        Underpass,
        Overpass
    }

    public enum BridgeTriggerDirection
    {
        In,
        Out
    }

    public enum AIState
    {
        Idle,
        Driving,
        Reversing
    }
}