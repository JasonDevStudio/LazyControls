1. C# 软件开发
2. string[] values = new string[] {"3.1425","3","6.36","9.66"}， 元素可能有500个，
3. 编写一个公共函数 InferMainType(string[] values) 用来推断出来是 double[] 还是 int[] 还是 bool[] 还是 Datetime[]
4. 优先判断 long.TryParse("-1.23E-4", NumberStyles.Number|NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out var _val) 通过的为 long 数据类型
5. 其次判断 double.TryParse("-1.23E-4", NumberStyles.Number|NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out var _val) 通过的为 double 数据类型
6. 再次判断 DateTime.TryParse(value, out _) 通过的为时间类型
7. 再次判断 bool.TryParse(value, out _) 通过的为 bool 类型
8. 上述都不满足的，默认为 string 类型
9. 转换之前剔除空字符串或者空值
10.声明 isall，默认为false， 如果剔除空值之后所有转换都成功的话，isall = true;
11.如果 isall = true, 则返回这个数据类型， 如果!=true,则调用 InferMainType函数，进行下一类型的数据转换和推断。
12.如果 long,double,datetime，bool 转换都得不到 isall = true; 就推断为 string 类型
