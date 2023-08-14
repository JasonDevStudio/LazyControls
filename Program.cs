// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;

Random random = new Random();
string[] values = new string[5000000];

// 随机生成数值
for (int i = 0; i < values.Length; i++)
{
    double number = random.NextDouble() * 10 + random.Next(0, 100);
    values[i] = number.ToString("E", CultureInfo.InvariantCulture);
}

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

// 调用 InferMainType 函数
Type mainType = InferMainType(values);

stopwatch.Stop();

Console.WriteLine($"主要类型: {mainType}");
Console.WriteLine($"耗时: {stopwatch.ElapsedMilliseconds} 毫秒");

static Type InferMainType(string[] values)
{
    bool isAll;

    // 剔除空字符串或空值
    values = Array.FindAll(values, s => !string.IsNullOrEmpty(s));

    // 优先判断 long 数据类型
    isAll = true;
    foreach (var value in values)
    {
        if (!long.TryParse(value, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out _))
        {
            isAll = false;
            break;
        }
    }
    if (isAll) return typeof(long);

    // 其次判断 double 数据类型
    isAll = true;
    foreach (var value in values)
    {
        if (!double.TryParse(value, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out _))
        {
            isAll = false;
            break;
        }
    }
    if (isAll) return typeof(double);

    // 再次判断 DateTime 类型
    isAll = true;
    foreach (var value in values)
    {
        if (!DateTime.TryParse(value, out _))
        {
            isAll = false;
            break;
        }
    }
    if (isAll) return typeof(DateTime);

    // 再次判断 bool 类型
    isAll = true;
    foreach (var value in values)
    {
        if (!bool.TryParse(value, out _))
        {
            isAll = false;
            break;
        }
    }
    if (isAll) return typeof(bool);

    // 如果上述都不满足，则推断为 string 类型
    return typeof(string);
}