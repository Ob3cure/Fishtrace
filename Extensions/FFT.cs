using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.VisualBasic;

namespace Fishtrace;

public static class FFT
{
    public static int FFTwindow = 20;
    public static double ReduceRatio = 0.9;

    public static void ChangeRatio03()
    {
        ReduceRatio = 0.3;
    }

    public static void ChangeRatio04()
    {
        ReduceRatio = 0.4;
    }

    public static void ChangeRatio05()
    {
        ReduceRatio = 0.5;
    }

    public static void ChangeRatio06()
    {
        ReduceRatio = 0.6;
    }

    public static void ChangeRatio09()
    {
        ReduceRatio = 0.9;
    }

    public static List<double> ZeroPadding(List<double> l)
    {
        int n = l.Count();
        int u = NextPowerOfTwoOrZero(n);
        List<double> L = new List<double>(l);
        if (!(u == 0))
        {
            for (int i = n; i < u; i++)
            {
                L.Add(0);
            }
        }

        return L;
    }

    public static int NextPowerOfTwoOrZero(int n)
    {
        if (n <= 0) return 0;
        if ((n & (n - 1)) == 0) return 0;
        int p = 1;
        while (p < n) p <<= 1;
        return p;
    }

    //e^{ix}
    public static Complex Exp(double x)
    {
        Complex c = new Complex(Math.Cos(x), Math.Sin(x));
        return c;
    }

    public static bool isEven(double x)
    {
        if (2 * ((int)(x / 2)) == x) { return true; }
        return false;
    }

    public static List<double> SquareTheList(List<double> x)
    {
        List<double> X = new List<double>(x);
        for (int i = 0; i < x.Count(); i++)
        {
            X[i] = X[i] * X[i];
        }

        return X;
    }

    public static List<Complex> SquareTheList(List<Complex> x)
    {
        List<Complex> X = new List<Complex>(x);
        for (int i = 0; i < x.Count(); i++)
        {
            X[i] = X[i] * X[i];
        }

        return X;
    }


    public static List<Complex> Time2Freq(List<double> x)
    {
        int n = x.Count(); //Should be the power of 2
        if (n == 1) { return new List<Complex> { x[0] }; }

        List<double> even = new List<double>();
        List<double> odd = new List<double>();
        for (int i = 0; i < n; i++)
        {
            if (isEven(i))
            {
                even.Add(x[i]);
            }
            else
            {
                odd.Add(x[i]);
            }
        }

        List<Complex> even_processed = Time2Freq(even);
        List<Complex> odd_processed = Time2Freq(odd);

        List<Complex> w = new List<Complex>();
        for (int i = 0; i < n / 2; i++)
        {
            double omega = 2 * Math.PI / n;
            w.Add(Exp(-1 * omega * i));
        }

        List<Complex> freq = new List<Complex>();
        for (int k = 0; k < n / 2; k++)
        {
            freq.Add(even_processed[k] + w[k] * odd_processed[k]);
        }
        for (int k = 0; k < n / 2; k++)
        {
            freq.Add(even_processed[k] - w[k] * odd_processed[k]);
        }

        return freq;
    }

    public static List<Complex> Time2Freq(List<Complex> x)
    {
        int n = x.Count(); //Should be the power of 2
        if (n == 1) { return new List<Complex> { x[0] }; }

        List<Complex> even = new List<Complex>();
        List<Complex> odd = new List<Complex>();
        for (int i = 0; i < n; i++)
        {
            if (isEven(i))
            {
                even.Add(x[i]);
            }
            else
            {
                odd.Add(x[i]);
            }
        }

        List<Complex> even_processed = Time2Freq(even);
        List<Complex> odd_processed = Time2Freq(odd);

        List<Complex> w = new List<Complex>();
        for (int i = 0; i < n / 2; i++)
        {
            double omega = 2 * Math.PI / n;
            w.Add(Exp(-1 * omega * i));
        }

        List<Complex> freq = new List<Complex>();
        for (int k = 0; k < n / 2; k++)
        {
            freq.Add(even_processed[k] + w[k] * odd_processed[k]);
        }
        for (int k = 0; k < n / 2; k++)
        {
            freq.Add(even_processed[k] - w[k] * odd_processed[k]);
        }

        return freq;
    }

    public static List<Complex> Freq2Time(List<Complex> x)
    {
        int n = x.Count();
        for (int i = 0; i < n; i++)
        {
            x[i] = Complex.Conjugate(x[i]);
        }

        List<Complex> freq = Time2Freq(x);
        for (int i = 0; i < n; i++)
        {
            freq[i] = Complex.Conjugate(freq[i] / n);
        }

        return freq;
    }

    public static List<double> GetReal(List<Complex> freq)
    {
        return freq.Select(c => c.Real).ToList();
    }

    public static Animation? ApplyFFT(Animation animation, int startframe, int endframe)
    {
        // 1. 提取指定范围内的帧，按索引顺序排列
        var frames = animation.FrameCollection
            .Where(kvp => kvp.Key >= startframe && kvp.Key <= endframe)
            .OrderBy(kvp => kvp.Key)
            .ToList();

        if (frames.Count == 0) return null;

        // 2. 提取坐标、外观和原始键
        int n = frames.Count;
        List<double> xList = new List<double>(n);
        List<double> yList = new List<double>(n);
        List<string> cosmeticList = new List<string>(n);
        List<int> keys = new List<int>(n);

        foreach (var kvp in frames)
        {
            keys.Add(kvp.Key);
            var (x, y, cosmetic,v) = Animation.Translate(kvp.Value);
            xList.Add(x);
            yList.Add(y);
            cosmeticList.Add(cosmetic ?? "null");
        }

        // 3. 记录原始数据的最大值和最小值（用于后续缩放）
        double minX = xList.Min();
        double maxX = xList.Max();
        double minY = yList.Min();
        double maxY = yList.Max();
        double deltaX = maxX - minX;
        double deltaY = maxY - minY;

        // 4. 补零到下一个 2 的幂
        List<double> xPadded = ZeroPadding(xList);
        List<double> yPadded = ZeroPadding(yList);
        int N = xPadded.Count;

        // 5. FFT
        List<Complex> xFreq = Time2Freq(xPadded);
        List<Complex> yFreq = Time2Freq(yPadded);

        // 6. 低通滤波（保留前 ReduceRatio 比例）
        int ratio = (int)(N * ReduceRatio);
        if (ratio == 0) return null;
        for (int i = ratio; i < N; i++)
        {
            xFreq[i] = Complex.Zero;
            yFreq[i] = Complex.Zero;
        }

        // 7. 逆变换
        List<Complex> xTime = Freq2Time(xFreq);
        List<Complex> yTime = Freq2Time(yFreq);

        // 取实部，截取原始长度
        List<double> xFiltered = xTime.Take(n).Select(c => c.Real).ToList();
        List<double> yFiltered = yTime.Take(n).Select(c => c.Real).ToList();

        // 8. 线性映射到原始范围
        double minXf = xFiltered.Min();
        double maxXf = xFiltered.Max();
        double minYf = yFiltered.Min();
        double maxYf = yFiltered.Max();

        // 避免除零（如果滤波后所有点相同，保持原值不变）
        if (maxXf - minXf != 0)
        {
            double scaleX = deltaX / (maxXf - minXf);
            for (int i = 0; i < n; i++)
                xFiltered[i] = (xFiltered[i] - minXf) * scaleX + minX;
        }
        // 如果原始范围 deltaX 为 0，说明所有原始 x 相同，保持滤波后也不变（设均为 minX）
        else if (deltaX == 0)
        {
            for (int i = 0; i < n; i++) xFiltered[i] = minX;
        }

        if (maxYf - minYf != 0)
        {
            double scaleY = deltaY / (maxYf - minYf);
            for (int i = 0; i < n; i++)
                yFiltered[i] = (yFiltered[i] - minYf) * scaleY + minY;
        }
        else if (deltaY == 0)
        {
            for (int i = 0; i < n; i++) yFiltered[i] = minY;
        }

        // 9. 构建新动画
        Animation newAnim = new Animation(animation.FrameRate, animation.Name + "_filtered")
        {
            Relative = animation.Relative,
            initX = animation.initX,
            initY = animation.initY,
            Cosmetics = new List<string>(animation.Cosmetics)
        };

        // 10. 填充帧字典（原样复制未处理帧，替换处理帧）
        foreach (var kvp in animation.FrameCollection)
        {
            int key = kvp.Key;
            if (key >= startframe && key <= endframe)
            {
                int index = keys.IndexOf(key);
                if (index >= 0)
                {
                    double newX = xFiltered[index];
                    double newY = yFiltered[index];
                    string cosmetic = cosmeticList[index];
                    newAnim.FrameCollection[key] = $"x:{newX},y:{newY},Cosmetic:{cosmetic}";
                }
            }
            else
            {
                newAnim.FrameCollection[key] = kvp.Value;
            }
        }

        return newAnim;
    }
}