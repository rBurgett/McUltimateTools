using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Connections;

namespace McUltimateTools;

public class Db
{
    private string usersTableName;
    private AmazonDynamoDBClient client;

    public Db(string usersTableName)
    {
        this.usersTableName = usersTableName;
        client = new AmazonDynamoDBClient();
    }

    public async Task<User> AddUser(User user)
    {
        // if id is empty
        if (user.Id == "")
        {
            throw new Exception("user.Id is empty");
        }
        var now = Util.GetDateString();
        user.CreatedAt = now;
        user.UpdatedAt = now;
        var request = new PutItemRequest
        {
            TableName = usersTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                {"Id", new AttributeValue {S = user.Id}},
                {"Email", new AttributeValue {S = user.Email}},
                {"CreatedAt", new AttributeValue {S = user.CreatedAt}},
                {"UpdatedAt", new AttributeValue {S = user.UpdatedAt}},
            }
        };
        await client.PutItemAsync(request);
        return user;
    }

    public async Task<string> DeleteUser(string id)
    {
        var request = new DeleteItemRequest
        {
            TableName = usersTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                {"Id", new AttributeValue {S = id}},
            }
        };
        await client.DeleteItemAsync(request);
        return id;
    }

    public async Task<User[]> GetUsers()
    {
        var request = new ScanRequest
        {
            TableName = usersTableName,
        };
        var response = await client.ScanAsync(request);
        var users = response.Items.Select(item =>
        {
            var id = item["Id"].S;
            var email = item["Email"].S;
            var createdAt = item["CreatedAt"].S;
            var updatedAt = item["UpdatedAt"].S;
            return new User(id, email, createdAt, updatedAt);
        }).ToArray();
        return users;
    }

    public async Task<User?> GetUser(string id)
    {
        var request = new GetItemRequest
        {
            TableName = usersTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                {"Id", new AttributeValue {S = id}},
            }
        };
        var response = await client.GetItemAsync(request);
        if (response.Item.Count == 0)
        {
            return null;
        }
        var item = response.Item;
        var email = item["Email"].S;
        var createdAt = item["CreatedAt"].S;
        var updatedAt = item["UpdatedAt"].S;
        return new User(id, email, createdAt, updatedAt);
    }

}
