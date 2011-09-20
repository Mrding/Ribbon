项目定义：

		Luna.Core：项目将会作为核心项目。包含一些基础底层扩展和各种增强型数据结构，重点在底层框架。这样其他人的项目可以用。
							  不要与IOC框架或者NH有任何联系，此类库只引用.net FCL和DyanmicUtilitily（反射增强）类库。
		Luna.Common：项目会仅仅作为一些通用的Entity和接口来给其他项目共享数据结构，重点在业务交互。
									  此项目会引用具体的IOC框架，并做相应的扩展。
									  
类库阐述：

		Luna.Core命名空间：
				
				ApplicationCache：一个全局字典，放一些登录用户、权限等信息。
				Lazy：延迟加载辅助类。
				ParallelUtility：并行切割工具类，根据CPU核心数来分解任务。
				Tuple：元组。一个拥有N个属性的数据结构，用在方法的返回值，可以返回多个返回值。
				EnumTypeConverter：枚举转换器。
				
		Luna.Core.Extentions命名空间：
		
				ArrayExtension：对数组的扩展。
				CommonExtension：对普通对象的扩展，比如是否为空等，字符串是否为Null or Empty等。
				ComparableExtension：比较对象的扩展。
				DateTimeExtension：时间日期的扩展。
				EnumerableExtension：集合的扩展。
				ExceptionExtension：异常的扩展。
				ReflectionExtension：反射的扩展。
				TypeExtension：类型的扩展。