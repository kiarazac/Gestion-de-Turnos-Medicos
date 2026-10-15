$conn = New-Object System.Data.SqlClient.SqlConnection('Server=localhost\SQLEXPRESS;Database=dbGestionTurnos;Integrated Security=True;TrustServerCertificate=True;')
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = @"
UPDATE Usuarios 
SET Contrasena = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CAST(Contrasena AS VARCHAR(255))), 2))
WHERE LEN(Contrasena) <> 64 OR Contrasena LIKE '%[^0-9a-fA-F]%';
"@
$rowsAffected = $cmd.ExecuteNonQuery()
Write-Host "Usuarios actualizados a hash SHA-256: $rowsAffected"

# Listar usuarios para verificar
$cmd.CommandText = "SELECT IdUsuario, Nombre, Correo, Contrasena FROM Usuarios"
$r = $cmd.ExecuteReader()
while ($r.Read()) {
    Write-Host "$($r[0]) | $($r[1]) | $($r[2]) | $($r[3])"
}
$conn.Close()
