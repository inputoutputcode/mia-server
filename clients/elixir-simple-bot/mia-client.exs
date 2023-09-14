#!/usr/bin/env elixir

defmodule UDP do

  def open() do
    case :gen_udp.open(9001) do
      {:ok, socket}    -> IO.puts "Opened an UDP socket at #{inspect socket}"
                          socket
      {:error, reason} -> IO.puts "Error '#{inspect reason}' opening an UDP socket"
                          nil
      _other           -> IO.puts "Error opening an UDP socket"
                          nil
    end
  end

  def send_msg(socket, msg) do
    :gen_udp.send(socket, {192,168,0,10}, 5500, msg)
  end

  def receiver() do
    
    receive do
      {:udp, _socket, _ip, _port, message} ->
#        IO.puts "Got the UDP message #{IO.ANSI.yellow}'#{message}'#{IO.ANSI.reset} from socket #{inspect _socket} at #{inspect _ip}:#{_port}"
        message |> to_string
                |> String.split(";")
    after
      250 -> nil
    end
  end

  def close(socket) do
    :gen_udp.close(socket)
    IO.puts "Closed the UDP socket at #{inspect socket}"
  end

end

defmodule MiaClient do

  def start(reg_name \\ "Elixir Starter Bot") do
    IO.puts "Elixir Starter Bot as #{reg_name}"
    socket = UDP.open()
    UDP.send_msg(socket, "REGISTER;#{reg_name}")
    case UDP.receiver() do
      ["REGISTERED"] -> IO.puts "Successfully registered"
                        play_game(socket, nil)
      ["REJECTED"]   -> IO.puts "Server doesn't like me :-("
                        exit(1)
      _other         -> start
    end
  end

  def play_game(socket, current_bet) do
    case UDP.receiver do
      ["ROUND_STARTING", token]         -> IO.puts "Round started, my token is #{token}"
                                           UDP.send_msg socket, "JOIN_ROUND;#{token}"
                                           play_game socket, nil
      ["ROUND_STARTED", runde, spieler] -> IO.puts "New round no. #{runde} with players #{spieler}"
                                           play_game socket, nil
      ["ROUND_CANCELED", reason]        -> IO.puts "Round was canceled for reason #{reason}"
                                           play_game socket, nil
      ["YOUR_TURN", token]              -> IO.puts "#{IO.ANSI.green}It is my turn!#{IO.ANSI.reset}"
                                           my_turn socket, token, current_bet
                                           play_game socket, current_bet
      ["PLAYER_ROLLS", name]            -> IO.puts "Player #{name} rolls the dice..."
                                           play_game socket, current_bet
      ["ANNOUNCED", name, dice]         -> IO.puts "Player #{name} announced a roll of #{dice}"
                                           play_game socket, dice

      ["PLAYER_WANTS_TO_SEE", name]     -> IO.puts "Player #{name} wants to see"
                                           play_game socket, current_bet
      ["ACTUAL_DICE", dice]             -> IO.puts "Dice are actually #{dice}"
                                           play_game socket, current_bet
      ["PLAYER_LOST",names, reason]     -> IO.puts "Player #{names} lost for reason #{reason}"
                                           play_game socket, nil
      ["SCORE", points]          	-> IO.puts "Scores are #{points}"
                                           play_game socket, nil
      _other                            -> play_game socket, current_bet
    end
  end

  def my_turn(socket, token, current_bet) do
    case current_bet do
      nil    -> UDP.send_msg socket, "ROLL;#{token}"
                handle_myroll socket
      _other -> UDP.send_msg socket, "SEE;#{token}"
    end
  end

  def handle_myroll(socket) do
    case UDP.receiver do
      ["ROLLED", dice, token]  -> UDP.send_msg socket, "ANNOUNCE;#{dice};#{token}"
      ["PLAYER_ROLLS", name]   -> IO.puts "I (#{name}) roll the dice..."
                                  handle_myroll socket
      _other                   -> nil
    end
  end

end

MiaClient.start()