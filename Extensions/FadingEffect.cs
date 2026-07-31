using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.Collections.Generic;

namespace Fishtrace;

/// <summary>
/// 提供 UIElement 透明度淡入淡出动画，支持暂停/恢复。
/// </summary>
public static class FadingEffect
{
    private class AnimationState
    {
        public Visual Visual { get; set; }
        public float TargetOpacity { get; set; }
        public bool IsPaused { get; set; }
        public CompositionAnimation CurrentAnimation { get; set; }
        public CompositionScopedBatch Batch { get; set; }
        public float CurrentOpacity => (float)Visual.Opacity;
    }

    private static readonly Dictionary<UIElement, AnimationState> _states = new Dictionary<UIElement, AnimationState>();

    /// <summary>
    /// 逐渐显现（透明度从当前值变为 1）
    /// </summary>
    public static void FadeIn(UIElement element, double durationMs = 300, Action? a = null)
    {
        AnimateTo(element, 1.0f, durationMs, a);
    }

    /// <summary>
    /// 逐渐虚化（透明度从当前值变为 0）
    /// </summary>
    public static void FadeOut(UIElement element, double durationMs = 300, Action? a = null)
    {
        AnimateTo(element, 0.0f, durationMs, a);
    }

    private static void AnimateTo(UIElement element, float targetOpacity, double durationMs, Action? a = null)
    {
        if (element == null) return;
        // 如果该元素正在播放动画（包括暂停状态），先停止并清除
        if (_states.TryGetValue(element, out var s))
        {
            return;
        }

        var visual = ElementCompositionPreview.GetElementVisual(element);
        float current = (float)visual.Opacity;
        if (Math.Abs(current - targetOpacity) < 0.001f)
        {
            visual.Opacity = targetOpacity;
            return;
        }

        var compositor = visual.Compositor;
        var anim = compositor.CreateScalarKeyFrameAnimation();
        anim.InsertKeyFrame(0.0f, current);
        anim.InsertKeyFrame(1.0f, targetOpacity);
        anim.Duration = TimeSpan.FromMilliseconds(durationMs);

        var state = new AnimationState
        {
            Visual = visual,
            TargetOpacity = targetOpacity,
            IsPaused = false,
            CurrentAnimation = anim,
        };
        _states[element] = state;

        var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
        batch.Completed += (s, e) =>
        {
            visual.Opacity = targetOpacity;
            visual.StopAnimation("Opacity");
            batch.Dispose();
            if (_states.TryGetValue(element, out var st) && st == state)
            {
                _states.Remove(element);
            }

            if (a != null)
            {
                a();
            }
        };
        state.Batch = batch;
        visual.StartAnimation("Opacity", anim);
        batch.End();
    }
}
