using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

public static class MongoDbHelper
{
	private static readonly IMongoDatabase _database;
	private static readonly IMongoDatabase _database2;
	private static readonly MongoClient _client;

	static MongoDbHelper()
	{
		try
		{
			// 从配置文件中读取连接参数
			var connectionString = ConfigurationManager.AppSettings["MongoConnStr"];
			var databaseName = ConfigurationManager.AppSettings["MongoDb"];
			var databaseName2 = ConfigurationManager.AppSettings["MongoDb_PLC"];//工程名_PLC//目前固定

			//if (string.IsNullOrEmpty(connectionString))
			//{
			//	// 如果没有配置MongoConnStr，则使用单独的参数构建连接字符串
			//	var mongoIp = ConfigurationManager.AppSettings["MIP1"];
			//	var mongoPort = ConfigurationManager.AppSettings["MPORT1"];
			//	var mongoUser = ConfigurationManager.AppSettings["MUSER1"];
			//	var mongoPwd = ConfigurationManager.AppSettings["MPWD1"];
			//	var mongoDb = ConfigurationManager.AppSettings["MDB1"];

			//	connectionString = BuildConnectionString(mongoIp, mongoPort, mongoUser, mongoPwd);
			//	databaseName = string.IsNullOrEmpty(mongoDb) ? "admin" : mongoDb;
				
			//}

			// 配置MongoClientSettings
			var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
			settings.MaxConnectionPoolSize = 100; // 连接池大小
			settings.ConnectTimeout = TimeSpan.FromSeconds(30); // 连接超时
			settings.SocketTimeout = TimeSpan.FromSeconds(60); // 套接字超时
			settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30); // 服务器选择超时

			_client = new MongoClient(settings);
			_database = _client.GetDatabase(databaseName);
			_database2 = _client.GetDatabase(databaseName2);

			Console.WriteLine("MongoDB连接初始化成功");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"MongoDB连接初始化失败: {ex.Message}");
			throw;
		}
	}

	private static string BuildConnectionString(string ip, string port, string user, string pwd)
	{
		if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pwd))
		{
			return $"mongodb://{ip}:{port}";
		}
		return $"mongodb://{user}:{pwd}@{ip}:{port}";
	}

	/// <summary>
	/// 获取集合
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <returns></returns>
	public static IMongoCollection<T> GetCollection<T>(string collectionName)
	{
		return _database.GetCollection<T>(collectionName);
	}
	public static IMongoCollection<T> GetCollectionTwo<T>(string collectionName)
	{
		return _database2.GetCollection<T>(collectionName);
	}

	/// <summary>
	/// 插入单个文档
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="document">文档对象</param>
	public static void InsertOne<T>(string collectionName, T document)
	{
		var collection = GetCollection<T>(collectionName);
		collection.InsertOne(document);
	}
	public static List<string> Distinctss<T>(string collectionName, string distinctName)
	{
		var collection = GetCollection<T>(collectionName);
		var cities = collection.Distinct<string>(distinctName, FilterDefinition<T>.Empty).ToList();
		return cities;
	}

	/// <summary>
	/// 插入多个文档
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="documents">文档列表</param>
	public static void InsertMany<T>(string collectionName, IEnumerable<T> documents)
	{
		var collection = GetCollection<T>(collectionName);
		collection.InsertMany(documents);
	}

	/// <summary>
	/// 查询所有文档
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <returns></returns>
	public static List<T> FindAll<T>(string collectionName)
	{
		var collection = GetCollection<T>(collectionName);
		return collection.Find(_ => true).ToList();
	}

	/// <summary>
	/// 根据条件查询文档
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="filter">过滤条件</param>
	/// <returns></returns>
	public static List<T> Find<T>(string collectionName, FilterDefinition<T> filter)
	{
		var collection = GetCollection<T>(collectionName);
		return collection.Find(filter).ToList();
	}

	/// <summary>
	/// 根据条件查询文档(带排序和分页)
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="filter">过滤条件</param>
	/// <param name="sort">排序条件</param>
	/// <param name="pageIndex">页码(从1开始)</param>
	/// <param name="pageSize">每页条数</param>
	/// <returns></returns>
	public static List<T> Find<T>(string collectionName,
	                              FilterDefinition<T> filter,
	                              SortDefinition<T> sort = null,
	                              int pageIndex = 1,
	                              int pageSize = 10)
	{
		var collection = GetCollection<T>(collectionName);
		var query = collection.Find(filter);

		if (sort != null)
		{
			query = query.Sort(sort);
		}

		return query.Skip((pageIndex - 1) * pageSize)
			.Limit(pageSize)
			.ToList();
	}

	/// <summary>
	/// 根据ID查询单个文档
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="id">文档ID</param>
	/// <returns></returns>
	public static T FindById<T>(string collectionName, string id)
	{
		var collection = GetCollection<T>(collectionName);
		var filter = Builders<T>.Filter.Eq("_id", id);
		return collection.Find(filter).FirstOrDefault();
	}

	/// <summary>
	/// 更新单个文档
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="id">文档ID</param>
	/// <param name="update">更新定义</param>
	/// <returns>是否更新成功</returns>
	public static bool UpdateOne<T>(string collectionName, string id, UpdateDefinition<T> update)
	{
		var collection = GetCollection<T>(collectionName);
		var filter = Builders<T>.Filter.Eq("_id", id);
		var result = collection.UpdateOne(filter, update);
		return result.ModifiedCount > 0;
	}

	/// <summary>
	/// 更新多个文档
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="filter">过滤条件</param>
	/// <param name="update">更新定义</param>
	/// <returns>更新的文档数量</returns>
	public static long UpdateMany<T>(string collectionName, FilterDefinition<T> filter, UpdateDefinition<T> update)
	{
		var collection = GetCollection<T>(collectionName);
		var result = collection.UpdateMany(filter, update);
		return result.ModifiedCount;
	}

	/// <summary>
	/// 替换单个文档
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="id">文档ID</param>
	/// <param name="document">新文档</param>
	/// <returns>是否替换成功</returns>
	public static bool ReplaceOne<T>(string collectionName, string id, T document)
	{
		var collection = GetCollection<T>(collectionName);
		var filter = Builders<T>.Filter.Eq("_id", id);
		var result = collection.ReplaceOne(filter, document);
		return result.ModifiedCount > 0;
	}

	/// <summary>
	/// 删除单个文档
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="id">文档ID</param>
	/// <returns>是否删除成功</returns>
	public static bool DeleteOne<T>(string collectionName, string id)
	{
		var collection = GetCollection<T>(collectionName);
		var filter = Builders<T>.Filter.Eq("_id", id);
		var result = collection.DeleteOne(filter);
		return result.DeletedCount > 0;
	}

	/// <summary>
	/// 删除多个文档
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="filter">过滤条件</param>
	/// <returns>删除的文档数量</returns>
	public static long DeleteMany<T>(string collectionName, FilterDefinition<T> filter)
	{
		var collection = GetCollection<T>(collectionName);
		var result = collection.DeleteMany(filter);
		return result.DeletedCount;
	}

	/// <summary>
	/// 统计文档数量
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="filter">过滤条件</param>
	/// <returns>文档数量</returns>
	public static long Count<T>(string collectionName, FilterDefinition<T> filter = null)
	{
		var collection = GetCollection<T>(collectionName);
		return filter == null ?
			collection.CountDocuments(_ => true) :
			collection.CountDocuments(filter);
	}

	/// <summary>
	/// 创建索引
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="keys">索引键</param>
	/// <param name="options">索引选项</param>
	public static void CreateIndex<T>(string collectionName, IndexKeysDefinition<T> keys, CreateIndexOptions options = null)
	{
		var collection = GetCollection<T>(collectionName);
		var model = new CreateIndexModel<T>(keys, options);
		collection.Indexes.CreateOne(model);
	}

	/// <summary>
	/// 创建文本索引
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="field">字段名</param>
	public static void CreateTextIndex<T>(string collectionName, string field)
	{
		var collection = GetCollection<T>(collectionName);
		var keys = Builders<T>.IndexKeys.Text(field);
		var model = new CreateIndexModel<T>(keys);
		collection.Indexes.CreateOne(model);
	}

	/// <summary>
	/// 执行聚合查询
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="pipeline">聚合管道</param>
	/// <returns>聚合结果</returns>
	//public static List<T> Aggregate<T>(string collectionName, params PipelineStageDefinition<T, T>[] pipeline)
	//{
	//    var collection = GetCollection<T>(collectionName);
	//    return collection.Aggregate(new AggregateOptions { AllowDiskUse = true })
	//                    .AppendStages(pipeline)
	//                    .ToList();
	//}
	// <summary>
	/// 分页查询文档(带总数统计)
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="filter">过滤条件</param>
	/// <param name="sort">排序条件</param>
	/// <param name="pageIndex">页码(从1开始)</param>
	/// <param name="pageSize">每页条数</param>
	/// <returns>分页结果</returns>
	public static PageResult<T> FindByPageWithTotal<T>(string collectionName,
	                                                   FilterDefinition<T> filter = null,
	                                                   SortDefinition<T> sort = null,
	                                                   int pageIndex = 1,
	                                                   int pageSize = 10)
	{
		var collection = GetCollection<T>(collectionName);

		// 默认过滤条件
		if (filter == null)
		{
			filter = Builders<T>.Filter.Empty;
		}

		// 获取总记录数
		var totalCount = collection.CountDocuments(filter);

		// 构建查询
		var query = collection.Find(filter);

		// 添加排序条件
		if (sort != null)
		{
			query = query.Sort(sort);
		}

		// 执行分页查询
		var items = query.Skip((pageIndex - 1) * pageSize)
			.Limit(pageSize)
			.ToList();

		return new PageResult<T>
		{
			TotalCount = totalCount,
			Items = items,
			PageIndex = pageIndex,
			PageSize = pageSize
		};
	}
	public static PageResult<T> FindByPageWithDynamicFilters<T>(
		string collectionName,
		List<FilterDefinition<T>> andFilters = null,
		//List<FilterDefinition<T>> orFilters = null,
		SortDefinition<T> sort = null,
		int pageIndex = 1,
		int pageSize = 10)
	{
		var collection = GetCollection<T>(collectionName);

		// 默认查询所有记录
		var filter = Builders<T>.Filter.Empty;

		// 组合 AND 条件
		if (andFilters != null && andFilters.Any())
		{
			filter = Builders<T>.Filter.And(andFilters);
		}

		////// 组合 OR 条件
		////if (orFilters != null && orFilters.Any())
		////{
		////    filter = Builders<T>.Filter.Or(orFilters);
		////}

		//// 执行查询
		//var query = collection.Find(filter);

		//if (sort != null)
		//{
		//	query = query.Sort(sort);
		//}

		//var items = query.Skip((pageIndex - 1) * pageSize)
		//	.Limit(pageSize)
		//	.ToList();

		//var totalCount = collection.CountDocuments(filter);

		//return new PageResult<T>
		//{
		//	TotalCount = totalCount,
		//	Items = items,
		//	PageIndex = pageIndex,
		//	PageSize = pageSize
		//};
		var totalCount = collection.CountDocuments(filter);
		if (sort == null)
		{
			var items = collection.Find(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
			return new PageResult<T>
			{
				TotalCount = totalCount,
				Items = items,
				PageIndex = pageIndex,
				PageSize = pageSize
			};
		}
		else
		{
			var items = collection.Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
			return new PageResult<T>
			{
				TotalCount = totalCount,
				Items = items,
				PageIndex = pageIndex,
				PageSize = pageSize
			};
		}
	}
	/// <summary>
	/// 根据条件查询文档并返回指定字段
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="filter">过滤条件</param>
	/// <param name="projection">字段投影</param>
	/// <returns>包含指定字段的文档列表</returns>
	public static List<T> FindWithProjection<T>(
		string collectionName,
		FilterDefinition<T> filter,
		ProjectionDefinition<T> projection)
	{
		var collection = GetCollection<T>(collectionName);
		return collection.Find(filter)
			.Project<T>(projection)
			.ToList();
	}
	public static PageResult<T> FindByPageWithDynamicFilters2<T>(
		string collectionName,
		List<FilterDefinition<T>> andFilters = null,
		//List<FilterDefinition<T>> orFilters = null,
		SortDefinition<T> sort = null,
		int pageIndex = 1,
		int pageSize = 10)
	{
		var collection = GetCollectionTwo<T>(collectionName);

		// 默认查询所有记录
		var filter = Builders<T>.Filter.Empty;

		// 组合 AND 条件
		if (andFilters != null && andFilters.Any())
		{
			filter = Builders<T>.Filter.And(andFilters);
		}

		//// 组合 OR 条件
		//if (orFilters != null && orFilters.Any())
		//{
		//    filter = Builders<T>.Filter.Or(orFilters);
		//}

		//// 执行查询
		//var query = collection.Find(filter);

		//if (sort != null)
		//{
		//	query = query.Sort(sort);
		//}

		//var items = query.Skip((pageIndex - 1) * pageSize)
		//	.Limit(pageSize)
		//	.ToList();

		//var totalCount = collection.CountDocuments(filter);

		//return new PageResult<T>
		//{
		//	TotalCount = totalCount,
		//	Items = items,
		//	PageIndex = pageIndex,
		//	PageSize = pageSize
		//};
        var totalCount = collection.CountDocuments(filter);
        if (sort == null)
		{
            var items = collection.Find(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
            return new PageResult<T>
            {
                TotalCount = totalCount,
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
		else
        {
            var items = collection.Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
            return new PageResult<T>
            {
                TotalCount = totalCount,
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
	}
	/// <summary>
	/// 根据条件查询文档并返回指定字段(带分页)
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="filter">过滤条件</param>
	/// <param name="projection">字段投影</param>
	/// <param name="sort">排序条件</param>
	/// <param name="pageIndex">页码(从1开始)</param>
	/// <param name="pageSize">每页条数</param>
	/// <returns>包含指定字段的分页文档列表</returns>
	public static PageResult<T> FindWithProjectionByPage<T>(
		string collectionName,
		FilterDefinition<T> filter,
		ProjectionDefinition<T> projection,
		SortDefinition<T> sort = null,
		int pageIndex = 1,
		int pageSize = 10)
	{
		var collection = GetCollection<T>(collectionName);

		// 获取总记录数
		var totalCount = collection.CountDocuments(filter);

		// 构建查询
		var query = collection.Find(filter)
			.Project<T>(projection);

		// 添加排序条件
		if (sort != null)
		{
			query = query.Sort(sort);
		}

		// 执行分页查询
		var items = query.Skip((pageIndex - 1) * pageSize)
			.Limit(pageSize)
			.ToList();

		return new PageResult<T>
		{
			TotalCount = totalCount,
			Items = items,
			PageIndex = pageIndex,
			PageSize = pageSize
		};
	}

	/// <summary>
	/// 根据条件查询文档并返回指定字段(使用Lambda表达式)
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <typeparam name="TProjection">返回类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="filter">过滤条件</param>
	/// <param name="projection">字段投影表达式</param>
	/// <returns>包含指定字段的文档列表</returns>
	public static List<TProjection> FindWithLambdaProjection<T, TProjection>(
		string collectionName,
		FilterDefinition<T> filter,
		Func<T, TProjection> projection)
	{
		var collection = GetCollection<T>(collectionName);
		return collection.Find(filter)
			.Project(x => projection(x))
			.ToList();
	}

	/// <summary>
	/// 根据条件查询文档并返回指定字段(带分页，使用Lambda表达式)
	/// </summary>
	/// <typeparam name="T">文档类型</typeparam>
	/// <typeparam name="TProjection">返回类型</typeparam>
	/// <param name="collectionName">集合名称</param>
	/// <param name="filter">过滤条件</param>
	/// <param name="projection">字段投影表达式</param>
	/// <param name="sort">排序条件</param>
	/// <param name="pageIndex">页码(从1开始)</param>
	/// <param name="pageSize">每页条数</param>
	/// <returns>包含指定字段的分页文档列表</returns>
	public static PageResult<TProjection> FindWithLambdaProjectionByPage<T, TProjection>(
		string collectionName,
		FilterDefinition<T> filter,
		Func<T, TProjection> projection,
		SortDefinition<T> sort = null,
		int pageIndex = 1,
		int pageSize = 10)
	{
		var collection = GetCollection<T>(collectionName);

		// 获取总记录数
		var totalCount = collection.CountDocuments(filter);

		// 构建查询
		var query = collection.Find(filter)
			.Project(x => projection(x));

		// 添加排序条件
		if (sort != null)
		{
			query = query.Sort(sort);
		}

		// 执行分页查询
		var items = query.Skip((pageIndex - 1) * pageSize)
			.Limit(pageSize)
			.ToList();

		return new PageResult<TProjection>
		{
			TotalCount = totalCount,
			Items = items,
			PageIndex = pageIndex,
			PageSize = pageSize
		};
	}
}

public class PageResult<T>
{
	public long TotalCount { get; set; }
	public List<T> Items { get; set; }
	public int PageIndex { get; set; }
	public int PageSize { get; set; }
	public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}