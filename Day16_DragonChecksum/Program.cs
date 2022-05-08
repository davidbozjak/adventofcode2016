using System.Text;

//Test GenerateDragonStep:                              
//Console.WriteLine(GenerateDragonStep("1"));             // expected: 100
//Console.WriteLine(GenerateDragonStep("0"));             // expected: 001
//Console.WriteLine(GenerateDragonStep("11111"));         // expected: 11111000000
//Console.WriteLine(GenerateDragonStep("111100001010"));  // expected: 1111000010100101011110000

//Test Cehcksum
//Console.WriteLine(GetChecksum("110010110100"));           // Expected 100

// Test input data
//string inputData = "10000";
//int discSpace = 20;

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