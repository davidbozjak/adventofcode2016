using System.Text;

// Test input data
//string inputData = "10000";

// Real input data
string inputData = "10010000000110000";

Console.WriteLine($"Part 1: {GenerateDragonDataChecksumForLength(inputData, 272)}");
Console.WriteLine($"Part 2: {GenerateDragonDataChecksumForLength(inputData, 35651584)}");

static string GenerateDragonDataChecksumForLength(string inputData, int minLength)
{
    string data = inputData;

    while (data.Length < minLength)
    {
        data = GenerateDragonStep(data);
    }

    return GetChecksum(data[..minLength]);
}

static string GenerateDragonStep(string inputData)
{
    var b = new StringBuilder();

    for(int i = inputData.Length - 1; i >= 0; i--)
    {
        b.Append(inputData[i] == '1' ? '0' : '1');
    }

    return inputData + '0' + b.ToString();
}

static string GetChecksum(string data)
{
    var checkSum = new StringBuilder();

    for (int i = 0; i < data.Length; i+= 2)
    {
        var pair = data[i..(i + 2)];
        if (pair.Length != 2) throw new Exception();

        int sum = pair.Sum(w => w == '0' ? 0 : 1);

        if (sum == 1) checkSum.Append('0');
        else checkSum.Append('1');
    }

    if (checkSum.Length % 2 == 0)
    {
        return GetChecksum(checkSum.ToString());
    }
    else return checkSum.ToString();
}