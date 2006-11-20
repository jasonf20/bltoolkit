using System;

using NUnit.Framework;

using BLToolkit.Mapping;
using BLToolkit.Mapping.MetadataProvider;
using BLToolkit.Reflection;
using BLToolkit.Data;

namespace Mapping.MetadataProvider
{
	[TestFixture]
	public class MetadataProviderTest
	{
		class CustomMetadataProvider : MapMetadataProvider
		{
			public override string GetFieldName(ObjectMapper mapper, MemberAccessor member, out bool isSet)
			{
				string name = string.Empty;

				foreach (char c in member.Name)
				{
					if (char.IsUpper(c))
					{
						if (name.Length > 0)
							name += '_';
						name += c;
					}
					else
					{
						name += char.ToUpper(c);
					}
				}

				isSet = true;

				return name;
			}
		}

		public class Person
		{
			public string FirstName;
			public string LastName;
		}

		void MapMetadataProvider_OnCreateProvider(MapMetadataProvider parentProvider)
		{
			parentProvider.AddProvider(new CustomMetadataProvider());
		}

		[Test]
		public void Test()
		{
			MapMetadataProvider.OnCreateProvider += new OnCreateProvider(MapMetadataProvider_OnCreateProvider);

			using (DbManager db = new DbManager())
			{
				Person p = (Person)db
					.SetCommand("SELECT '1' as FIRST_NAME, '2' as 'LAST_NAME'")
					.ExecuteObject(typeof(Person));

				Assert.AreEqual("1", p.FirstName);
				Assert.AreEqual("2", p.LastName);
			}
		}

		[TearDown]
		public void TearDown()
		{
			MapMetadataProvider.OnCreateProvider -= new OnCreateProvider(MapMetadataProvider_OnCreateProvider);
		}
	}
}