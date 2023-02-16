-- ід - Прізвище І.Б. - Назва робочого відділу
SELECT 
	M.Id,
	M.Surname 
		+ ' ' + SUBSTRING(M.Name, 1, 1) 
		+ '. ' + SUBSTRING(M.Secname, 1, 1) + '.' AS PIB,
	D.Name AS MainDep,
	COALESCE(S.Name, '--')  AS SecDep
FROM
	Managers AS M
	JOIN Departments AS D  ON M.Id_main_dep = D.Id
	LEFT JOIN Departments AS S  ON M.Id_sec_dep = S.Id

/*
A [INNER] JOIN B - за збігами A i B
A LEFT JOIN B - всі записи з А, якщо є збіги, то + В, інакше - NULL
A RIGHT JOIN B -   В +А/NULL
A, B - "CROSS JOIN" - кожен-до-кожного - всі можливі комбінації
*/