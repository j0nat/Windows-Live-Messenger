<!DOCTYPE html>
<html>
<head>
<link rel="stylesheet" type="text/css" href="style.css">
<link rel="shortcut icon" href="img/favicon.ico" />
<title>Windows Live Messenger</title>
</head>

<body>
<center>
<div id="MainWindow">
<div id="ClientImageDiv"></div>
<div id="RegisterDiv">
<center><b>
<?php

$keysize = 128;
function mc_encrypt($encrypt, $key, $iv)
{
    $td = mcrypt_module_open(MCRYPT_RIJNDAEL_128, '', MCRYPT_MODE_ECB, '');
    mcrypt_generic_init($td, $key, $iv);
    $encrypted = mcrypt_generic($td, $encrypt);
    $encode = base64_encode($encrypted);
    mcrypt_generic_deinit($td);
    mcrypt_module_close($td);
    return $encode;
}

function mc_decrypt($decrypt, $key, $iv)
{
    $decoded = base64_decode($decrypt);
    $td = mcrypt_module_open(MCRYPT_RIJNDAEL_128, '', MCRYPT_MODE_ECB, '');
    mcrypt_generic_init($td, $key, $iv);
    $decrypted = mdecrypt_generic($td, $decoded);
    mcrypt_generic_deinit($td);
    mcrypt_module_close($td);
    return trim($decrypted);
}

if (isset($_POST['submit']))
{
	include 'opendb.php';
	
	$name = $mysqli->real_escape_string($_POST['name']);
	$id = $mysqli->real_escape_string(strtolower($_POST['id']));
	$pass = $mysqli->real_escape_string($_POST['pass']);

	$query = ("SELECT id FROM Account WHERE id='$id'");
	$result = $mysqli->query($query);
	$row_cnt = $result->num_rows;
	
	if ($row_cnt == 0)
	{
		if (!preg_match('/[^A-Za-z0-9]/', $id))
		{
			if (strlen($id) <= 2 | strlen($id) >= 12)
			{
				echo "Pick an ID between 2 and 12 characters.";
			}
			else
			{
				// ID is OK... Is everything else OK?
				if (strlen($name) <= 2 | strlen($name) >= 20)
				{
					echo "Pick a name between 2 and 20 characters.";
				}
				else
				{
					// Name is OK...
					if (strlen($pass) <= 2 | strlen($pass) >= 20)
					{
						echo "Pick a password between 2 and 20 characters.";
					}
					else
					{
						// Name, ID and password is OK!
						$pass = mc_encrypt(stripslashes($pass), $encryption_key, $encryption_iv);
						
						$query = ("INSERT INTO account VALUES('$id', '$name', '$pass', '', '', '')");
						$mysqli->query($query);
						
						echo "Your user has been registered.";
					}
				}
			}
		}
		else
		{
			echo "Pick a different ID.";
		}
	}
	else
	{
		echo "Pick a different ID.";
	}
}
?>
</b></center>
<form action="" method="post">
<center>
  <div>
    <h1>Register</h1>
    <label>
      <span>Display Name</span><br><input id="name" type="text" name="name" maxlength="20" />
    </label>
<br>
    <label>
      <span>Login ID</span><br><input id="email" type="text" name="id" maxlength="12" />
    </label>
<br>
    <label>
      <span>Password</span><br><input id="email" type="password" name="pass" maxlength="20" />
    </label>
<br>
    <label>
      <input type="submit" value="submit" name="submit" style="width: 200px; height: 50px;" />
    </label>
  </div>
  </center>
</form>
</div>

<div id="DownloadLinkDiv">
<center><a href="#">DOWNLOAD BETA CLIENT</a></center>
</div>
</div>
</center>
</body>
</html>
