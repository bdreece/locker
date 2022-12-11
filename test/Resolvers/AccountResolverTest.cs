namespace Locker.Testing.Resolvers;

// public class AccountResolverTest
// {
//     private readonly DataContextMock _mock = new();
//     private readonly Query query = new();
// 
//     private const string accountFragment = @"
//         fragment account on Account {
//             id
//             dateCreated
//             dateLastUpdated
//             roleID
//             role
//             userID
//             user
//             tenantID
//             tenant
//         }
//     ";
// 
//     [Fact]
//     public async Task GetAccountsTest()
//     {
//         var expected = _mock.Accounts.ToString();
//         var got = await ServiceMock.ExecuteRequestAsync(b =>
//             b.SetQuery(@$"
//                 {accountFragment}
//                 query {{
//                     accounts {{
//                         ...account
//                     }}
//                 }}
//             "));
// 
//         Assert.Equal(expected, got);
//     }
// 
//     [Fact]
//     public void GetFirstAccountTest()
//     {
//         var expected = _mock.Accounts.First();
//         var got = query.GetFirstAccount(_mock.DataContext).FirstOrDefault();
// 
//         Assert.Equal(expected, got);
//     }
// 
//     [Fact]
//     public void GetUniqueAccountTest()
//     {
//         var expected = _mock.Accounts.First();
//         var got = query.GetUniqueAccount(_mock.DataContext)
//             .SingleOrDefault(a => a.ID == expected.ID);
// 
//         Assert.Equal(expected, got);
//     }
// }