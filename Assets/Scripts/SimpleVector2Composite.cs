using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// A 2D planar motion vector computed from an up+down button pair and a left+right
/// button pair.
/// </summary>
/// <remarks>
/// This composite allows to grab arbitrary buttons from a device and arrange them in
/// a D-Pad like configuration. Based on button presses, the composite will return a
/// normalized direction vector.
///
/// Opposing motions cancel each other out. Meaning that if, for example, both the left
/// and right horizontal button are pressed, the resulting horizontal movement value will
/// be zero.
/// </remarks>
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[Preserve]
[DisplayStringFormat("{up}/{left}/{down}/{right}")] // This results in WASD.
public class SimpleVector2Composite : InputBindingComposite<Vector2>
{
#if UNITY_EDITOR
    static SimpleVector2Composite()
    {
        // Trigger our RegisterBindingComposite code in the editor.
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        InputSystem.RegisterBindingComposite<SimpleVector2Composite>();
    }

    /// <summary>
    /// Binding for the button that up (i.e. <c>(0,1)</c>) direction of the vector.
    /// </summary>
    /// <remarks>
    /// This property is automatically assigned by the input system.
    /// </remarks>
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    [InputControl(layout = "Button")] public int up = 0;

    /// <summary>
    /// Binding for the button that down (i.e. <c>(0,-1)</c>) direction of the vector.
    /// </summary>
    /// <remarks>
    /// This property is automatically assigned by the input system.
    /// </remarks>
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    [InputControl(layout = "Button")] public int down = 0;

    /// <summary>
    /// Binding for the button that left (i.e. <c>(-1,0)</c>) direction of the vector.
    /// </summary>
    /// <remarks>
    /// This property is automatically assigned by the input system.
    /// </remarks>
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    [InputControl(layout = "Button")] public int left = 0;

    /// <summary>
    /// Binding for the button that right (i.e. <c>(1,0)</c>) direction of the vector.
    /// </summary>
    /// <remarks>
    /// This property is automatically assigned by the input system.
    /// </remarks>
    [InputControl(layout = "Button")] public int right = 0;

    private bool upPressedLastFrame;
    private bool downPressedLastFrame;
    private bool leftPressedLastFrame;
    private bool rightPressedLastFrame;
    private float upPressTimestamp;
    private float downPressTimestamp;
    private float leftPressTimestamp;
    private float rightPressTimestamp;

    /// <inheritdoc />
    public override Vector2 ReadValue(ref InputBindingCompositeContext context)
    {
        var upIsPressed = context.ReadValueAsButton(up);
        var downIsPressed = context.ReadValueAsButton(down);
        var leftIsPressed = context.ReadValueAsButton(left);
        var rightIsPressed = context.ReadValueAsButton(right);

        if (upIsPressed && !upPressedLastFrame) upPressTimestamp = Time.time;
        if (downIsPressed && !downPressedLastFrame) downPressTimestamp = Time.time;
        if (leftIsPressed && !leftPressedLastFrame) leftPressTimestamp = Time.time;
        if (rightIsPressed && !rightPressedLastFrame) rightPressTimestamp = Time.time;

        upPressedLastFrame = upIsPressed;
        downPressedLastFrame = downIsPressed;
        leftPressedLastFrame = leftIsPressed;
        rightPressedLastFrame = rightIsPressed;

        var (x, xTimestamp) = (leftIsPressed, rightIsPressed) switch
        {
            (false, false) => (0f, 0f),
            (true, false) => (-1, leftPressTimestamp),
            (false, true) => (1, rightPressTimestamp),
            (true, true) when rightPressTimestamp > leftPressTimestamp => (1, rightPressTimestamp),
            (true, true) when rightPressTimestamp < leftPressTimestamp => (-1, leftPressTimestamp),
            (true, true) => (0f, 0f)
        };

        var (y, yTimestamp) = (downIsPressed, upIsPressed) switch
        {
            (false, false) => (0f, 0f),
            (true, false) => (-1, downPressTimestamp),
            (false, true) => (1, upPressTimestamp),
            (true, true) when upPressTimestamp > downPressTimestamp => (1, upPressTimestamp),
            (true, true) when upPressTimestamp < downPressTimestamp => (-1, downPressTimestamp),
            (true, true) => (0f, 0f)
        };

        if (x == 0 || y == 0) return new Vector2(x, y);
        return xTimestamp > yTimestamp ? new Vector2(x, 0) : new Vector2(0, y);
    }

    /// <inheritdoc />
    public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
    {
        return ReadValue(ref context).sqrMagnitude;
    }
}
