using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace McUltimateTools;

public class Db
{
    private readonly string _usersTableName;
    private readonly string _sessionTokensTableName;
    private readonly AmazonDynamoDBClient _client;

    public Db(string usersTableName, string sessionTokensTableName)
    {
        this._usersTableName = usersTableName;
        this._sessionTokensTableName = sessionTokensTableName;
        _client = new AmazonDynamoDBClient();
    }

    public async Task<User> AddUser(User user)
    {
        var now = Util.GetDateString();
        user.CreatedAt = now;
        user.UpdatedAt = now;
        var request = new PutItemRequest
        {
            TableName = _usersTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                {"Id", new AttributeValue {S = user.Id}},
                {"Email", new AttributeValue {S = user.Email}},
                {"PasswordHash", new AttributeValue {S = user.PasswordHash}},
                {"PasswordSalt", new AttributeValue {S = user.PasswordSalt}},
                {"CreatedAt", new AttributeValue {S = user.CreatedAt}},
                {"UpdatedAt", new AttributeValue {S = user.UpdatedAt}},
            }
        };
        await _client.PutItemAsync(request);
        return user;
    }

    public async Task<string> DeleteUser(string id)
    {
        var request = new DeleteItemRequest
        {
            TableName = _usersTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                {"Id", new AttributeValue {S = id}},
            }
        };
        await _client.DeleteItemAsync(request);
        return id;
    }

    public async Task<User[]> GetUsers()
    {
        var request = new ScanRequest
        {
            TableName = _usersTableName,
        };
        var response = await _client.ScanAsync(request);
        var users = response.Items.Select(item =>
        {
            var id = item["Id"].S;
            var email = item["Email"].S;
            var passwordHash = item["PasswordHash"].S;
            var passwordSalt = item["PasswordSalt"].S;
            var createdAt = item["CreatedAt"].S;
            var updatedAt = item["UpdatedAt"].S;
            return new User(id, email, passwordHash, passwordSalt, createdAt, updatedAt);
        }).ToArray();
        return users;
    }

    public async Task<User?> GetUser(string id)
    {
        var request = new GetItemRequest
        {
            TableName = _usersTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                {"Id", new AttributeValue {S = id}},
            }
        };
        var response = await _client.GetItemAsync(request);
        if (response.Item.Count == 0)
        {
            return null;
        }
        var item = response.Item;
        var email = item["Email"].S;
        var passwordHash = item["PasswordHash"].S;
        var passwordSalt = item["PasswordSalt"].S;
        var createdAt = item["CreatedAt"].S;
        var updatedAt = item["UpdatedAt"].S;
        return new User(id, email, passwordHash, passwordSalt, createdAt, updatedAt);
    }

    public async Task<User?> FindUserByEmail(string email)
    {
        var request = new ScanRequest
        {
            TableName = _usersTableName,
            FilterExpression = "Email = :email",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {":email", new AttributeValue {S = email}},
            }
        };
        var response = await _client.ScanAsync(request);
        if (response.Items.Count == 0)
        {
            return null;
        }
        var item = response.Items[0];
        var id = item["Id"].S;
        var passwordHash = item["PasswordHash"].S;
        var passwordSalt = item["PasswordSalt"].S;
        var createdAt = item["CreatedAt"].S;
        var updatedAt = item["UpdatedAt"].S;
        return new User(id, email, passwordHash, passwordSalt, createdAt, updatedAt);
    }

    public async Task<SessionToken> AddSessionToken(SessionToken sessionToken)
    {
        var request = new PutItemRequest
        {
            TableName = _sessionTokensTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                {"Token", new AttributeValue {S = sessionToken.Token}},
                {"User", new AttributeValue {S = sessionToken.User}},
                {"Expiration", new AttributeValue {S = sessionToken.Expiration}},
            }
        };
        await _client.PutItemAsync(request);
        return sessionToken;
    }

    public async Task<string> DeleteSessionToken(string token)
    {
        var request = new DeleteItemRequest
        {
            TableName = _sessionTokensTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                {"Token", new AttributeValue {S = token}},
            }
        };
        await _client.DeleteItemAsync(request);
        return token;
    }

    public async Task<SessionToken?> GetSessionToken(string token)
    {
        var request = new GetItemRequest
        {
            TableName = _sessionTokensTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                {"Token", new AttributeValue {S = token}},
            }
        };
        var response = await _client.GetItemAsync(request);
        if (response.Item.Count == 0)
        {
            return null;
        }
        var item = response.Item;
        var user = item["User"].S;
        var expiration = item["Expiration"].S;
        return new SessionToken(token, user, expiration);
    }
}
