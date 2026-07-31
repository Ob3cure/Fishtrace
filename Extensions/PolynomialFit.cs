namespace Fishtrace;


public static class PolynomialFit
{
    public static int PolyFitWindow = 200;
    public static int Degree = 3;

    public static void ChangeDegree02()
    {
        Degree = 2;
    }

    public static void ChangeDegree03()
    {
        Degree = 3;
    }

    public static void ChangeDegree05()
    {
        Degree = 5;
    }

    public static void ChangeDegree010()
    {
        Degree = 10;
    }

    public static void ChangeDegree020()
    {
        Degree = 20;
    }

    public static List<double> Inverse(List<double> matrix)
    {
        int degree = (int)Math.Pow(matrix.Count(), 0.5);

        double[,] aug = new double[degree, 2 * degree];
        for (int i = 0; i < degree; i++)
        {
            for (int j = 0; j < degree; j++)
                aug[i, j] = matrix[i * degree + j];
            aug[i, degree + i] = 1.0;
        }

        for (int col = 0; col < degree; col++)
        {
            int maxRow = col;
            for (int row = col + 1; row < degree; row++)
                if (Math.Abs(aug[row, col]) > Math.Abs(aug[maxRow, col]))
                    maxRow = row;


            if (Math.Abs(aug[maxRow, col]) < 1e-12)
                throw new InvalidOperationException("矩阵奇异，无法求逆");

            if (maxRow != col)
            {
                for (int j = col; j < 2 * degree; j++)
                {
                    double temp = aug[col, j];
                    aug[col, j] = aug[maxRow, j];
                    aug[maxRow, j] = temp;
                }
            }

            double pivot = aug[col, col];
            for (int j = col; j < 2 * degree; j++)
                aug[col, j] /= pivot;

            for (int row = 0; row < degree; row++)
            {
                if (row == col) continue;
                double factor = aug[row, col];
                for (int j = col; j < 2 * degree; j++)
                    aug[row, j] -= factor * aug[col, j];
            }
        }

        List<double> inv = new List<double>(degree * degree);
        for (int i = 0; i < degree; i++)
            for (int j = 0; j < degree; j++)
                inv.Add(aug[i, degree + j]);

        return inv;
    }

    public static List<double> Fit(List<double> y, int degree)
    {
        int n = y.Count();
        List<double> x = new List<double>();
        for (int i = 1; i < n + 1; i++)
        {
            for (int j = 0; j < degree; j++)
            {
                x.Add(Math.Pow(i, j));
            }
        }

        List<double> xtx = new List<double>();
        for (int i = 0; i < degree; i++)
        {
            for (int j = 0; j < degree; j++)
            {
                xtx.Add(0);
                for (int k = 0; k < n; k++)
                {
                    xtx[i * degree + j] = xtx[i * degree + j] + x[i + k * degree] * x[j + k * degree];
                }
            }
        }

        xtx = Inverse(xtx);
        List<double> xtxxt = new List<double>();

        for (int i = 0; i < degree; i++)
        {
            for (int j = 0; j < n; j++)
            {
                xtxxt.Add(0);
                for (int k = 0; k < degree; k++)
                {
                    xtxxt[i * n + j] = xtxxt[i * n + j] + xtx[i * degree + k] * x[j * degree + k];
                }
            }
        }

        List<double> xtxxty = new List<double>();
        for (int i = 0; i < degree; i++)
        {
            xtxxty.Add(0);
            for (int k = 0; k < n; k++)
            {
                xtxxty[i] = xtxxty[i] + xtxxt[i * n + k] * y[k];
            }
        }

        return xtxxty;
    }

    public static Animation ApplyPolynomialFit(Animation animation, int startframe, int endframe)
    {
        int degree = Degree; // 静态字段，预设阶数
        if (degree <= 0) return CloneAnimation(animation, "_copy");

        // 1. 提取指定范围内的帧
        var frames = animation.FrameCollection
            .Where(kvp => kvp.Key >= startframe && kvp.Key <= endframe)
            .OrderBy(kvp => kvp.Key)
            .ToList();

        int n = frames.Count;
        // 至少需要 2 个点才能进行线性拟合，否则直接返回副本
        if (n < 2)
            return CloneAnimation(animation, "_copy");

        // 2. 如果帧数小于预设的 degree，则用帧数作为 degree
        if (n < degree)
            degree = n;   // 此时 degree == n，即拟合 n-1 次多项式（系数个数为 n）

        // 确保 degree 至少为 1（但实际拟合时，degree=1 表示一次多项式，需要至少2个点）
        if (degree < 1) degree = 1;

        try
        {
            // 3. 提取坐标、外观和键
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

            // 4. 分别对 X 和 Y 进行多项式拟合（使用当前的 degree）
            List<double> coeffX = Fit(xList, degree);
            List<double> coeffY = Fit(yList, degree);

            // 5. 计算拟合值
            List<double> xFit = new List<double>(n);
            List<double> yFit = new List<double>(n);
            for (int t = 0; t < n; t++)
            {
                double xt = 0, yt = 0;
                for (int i = 0; i < degree; i++)
                {
                    xt += coeffX[i] * Math.Pow(t, i);
                    yt += coeffY[i] * Math.Pow(t, i);
                }
                xFit.Add(xt);
                yFit.Add(yt);
            }

            // 6. 构建新动画（复制属性）
            Animation newAnim = new Animation(animation.FrameRate, animation.Name + "_polynomial_fit")
            {
                Relative = animation.Relative,
                initX = animation.initX,
                initY = animation.initY,
                Cosmetics = new List<string>(animation.Cosmetics)
            };

            // 7. 填充帧字典
            foreach (var kvp in animation.FrameCollection)
            {
                int key = kvp.Key;
                if (key >= startframe && key <= endframe)
                {
                    int index = keys.IndexOf(key);
                    if (index >= 0)
                    {
                        double newX = xFit[index];
                        double newY = yFit[index];
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
        catch
        {
            // 拟合失败（如矩阵奇异），返回原始动画副本
            MainPage.Consoles.ConsoleOutput("PolyFit Apply Failed");
            return CloneAnimation(animation, "_copy");
        }
    }

    // 辅助：创建原始动画的副本（不变）
    private static Animation CloneAnimation(Animation animation, string suffix)
    {
        Animation copy = new Animation(animation.FrameRate, animation.Name + suffix)
        {
            Relative = animation.Relative,
            initX = animation.initX,
            initY = animation.initY,
            Cosmetics = new List<string>(animation.Cosmetics)
        };
        foreach (var kvp in animation.FrameCollection)
            copy.FrameCollection[kvp.Key] = kvp.Value;
        return copy;
    }
}