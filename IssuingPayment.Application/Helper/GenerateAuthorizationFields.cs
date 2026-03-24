namespace IssuingPayment.Application.Helper;

public class GenerateAuthorizationFields
{
    public static string GenerateAuthorizationCode()
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        char[] authCode = new char[6];

        authCode[0] = '0';

        for (int i = 1; i < authCode.Length; i++)
        {
            authCode[i] = chars[Random.Shared.Next(chars.Length)];
        }
        
        return new string(authCode);
    }
}